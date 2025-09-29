using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.CertificateRequests.Contracts;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.CertificateRequests.Features;

public class CertificateRequestService(IUnifiedDbContext db, CertificateService certificateService) {
    public async Task<KeyPair> CreateCertificateRequest(CreateCertificateRequestDTO requestDTO, string userId) {
        var user = await db.Users.Where(u => u.Role == Role.EeUser && u.Id == Guid.Parse(userId)).FirstAsync() ?? throw new Exception("EE user not found!");

        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        var subjectKeyPair = kpGen.GenerateKeyPair();

        var issuer = await db.Users.Where(u => u.Role == Role.CaUser && u.Id == Guid.Parse(requestDTO.SigningOrganization))
            .Include(u => u.MyCertificates).FirstAsync() ?? throw new Exception("Requested isser is not found!");

        if (!issuer.MyCertificates.Any(c => certificateService.GetStatus(c, null) == CertificateStatus.Active))
            throw new Exception("Requested isser is not able to sign certificates!");

        var minValidFrom = issuer.MyCertificates.Where(c => certificateService.GetStatus(c) == CertificateStatus.Active).Min(c => c.NotBefore);
        var maxValidUntil = issuer.MyCertificates.Where(c => certificateService.GetStatus(c) == CertificateStatus.Active).Max(c => c.NotAfter);

        if (requestDTO.NotBefore < minValidFrom)
            throw new Exception("NotBefore cannot be earlier than the issuer's earliest NotBefore!");
        if (requestDTO.NotAfter > maxValidUntil)
            throw new Exception("NotAfter cannot be later than the issuer's latest NotAfter!");
        if (requestDTO.NotBefore > requestDTO.NotAfter)
            throw new Exception("NotBefore cannot be later than the NotAfter!");

        CertificateRequest certificateRequest = CertificateRequestBuilder.CreateCertificateRequest(requestDTO, subjectKeyPair, issuer, user);

        await db.CertificateRequests.AddAsync(certificateRequest);
        await db.SaveChangesAsync();

        return new KeyPair {
            PublicKey = ToPem(subjectKeyPair.Public),
            PrivateKey = ToPem(subjectKeyPair.Private)
        };
    }

    private static string ToPem(AsymmetricKeyParameter key) {
        using var sw = new StringWriter();
        var pw = new PemWriter(sw);
        pw.WriteObject(key);
        pw.Writer.Flush();
        return sw.ToString();
    }

    public async Task CreateCertificateRequest(string signingUserId, string csr, DateTime? notAfter, string userId) {
        var user = await db.Users.Where(u => u.Role == Role.EeUser && u.Id == Guid.Parse(userId)).FirstAsync() ?? throw new Exception("EE user not found!");

        var issuer = await db.Users.Where(u => u.Role == Role.CaUser && u.Id == Guid.Parse(signingUserId))
            .Include(u => u.MyCertificates).FirstAsync() ?? throw new Exception("Requested isser is not found!");

        if (!issuer.MyCertificates.Any(c => certificateService.GetStatus(c, null) == CertificateStatus.Active))
            throw new Exception("Requested isser is not able to sign certificates!");

        var maxValidUntil = issuer.MyCertificates.Where(c => certificateService.GetStatus(c) == CertificateStatus.Active).Max(c => c.NotAfter);
        if (notAfter != null && notAfter > maxValidUntil)
            throw new Exception("NotAfter cannot be later than the issuer's latest NotAfter!");

        await db.CertificateRequests.AddAsync(new CertificateRequest {
            RequestedFor = user,
            RequestedFrom = issuer,
            EncodedCSR = csr,
            NotAfter = notAfter,
            SubmittedOn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task<List<CertificateRequestResponse>> GetCertificateRequests(string userId) {
        var user = await db.Users.Where(u => u.Role == Role.CaUser && u.Id == Guid.Parse(userId)).FirstAsync() ?? throw new Exception("CA user not found!");
        var requests = await db.CertificateRequests.Include(cr => cr.RequestedFrom).Where(cr => cr.RequestedFrom.Id == user.Id).ToListAsync();
        return [.. requests.Select(CertificateRequestDecoder.DecodeCertificateRequest)];
    }

    public async Task DeleteCertificateRequest(string userId, string requestId) {
        var user = await db.Users.Where(u => u.Role == Role.CaUser && u.Id == Guid.Parse(userId)).FirstAsync() ?? throw new Exception("CA user not found!");
        var request = await db.CertificateRequests.FindAsync(Guid.Parse(requestId)) ?? throw new Exception("Certificate request with given ID not found!");

        if (request.RequestedFrom.Id != user.Id)
            throw new Exception("This certificate is not requested from you!");

        var deleted = await db.CertificateRequests.Where(cr => cr.Id == request.Id).ExecuteDeleteAsync();

        if (deleted == 0)
            throw new Exception("Unable to delete certificate request with given ID!");
        if (deleted > 1)
            throw new Exception("Found multiple certificate requests with given ID!");
    }


    public async Task ApproveCertificateRequest(string userId, ApproveCertificateRequest approveRequest) {
        var user = await db.Users.Where(u => u.Role == Role.CaUser && u.Id == Guid.Parse(userId)).FirstAsync() ?? throw new Exception("CA user not found!");
        var request = await db.CertificateRequests.FindAsync(Guid.Parse(approveRequest.RequestId)) ?? throw new Exception("Certificate request with given ID not found!");

        if (request.RequestedFrom.Id != user.Id)
            throw new Exception("This certificate is not requested from you!");

        var publicKey = new Pkcs10CertificationRequest(Convert.FromBase64String(request.EncodedCSR)).GetPublicKey();
        await certificateService.CreateCertificate(approveRequest.RequestForm, false, userId, publicKey);
        await DeleteCertificateRequest(userId, approveRequest.RequestId);
    }
}