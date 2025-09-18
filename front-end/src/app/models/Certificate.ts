export interface Certificate {
  serialNumber: string;
  issuedBy: string;
  issuedTo: string;
  validFrom?: Date;
  validUntil?: Date;
  status: string;
  decryptedCertificate: string;
  canSign: boolean;
  pathLen: number;
}
