CREATE OR REPLACE FUNCTION truncate_tables() RETURNS void AS $$
DECLARE
    statements CURSOR FOR
        SELECT tablename, schemaname FROM pg_tables
        WHERE tableowner = 'postgres' AND schemaname = 'unified' AND tablename != '__EFMigrationsHistory';
BEGIN
    FOR stmt IN statements LOOP
            EXECUTE 'TRUNCATE TABLE ' || stmt.schemaname || '.' || quote_ident(stmt.tablename) || ' CASCADE;';
        END LOOP;
END;
$$ LANGUAGE plpgsql;
SELECT truncate_tables();

-- Admin user
INSERT INTO users (id, role, name, surname, organization, email, email_confirmed, hashed_password, refresh_token, refresh_token_expires_at)
VALUES
    ('d6e28a9d-354c-431c-8912-8c78b3b23a39', 'Admin', 'Michael', 'Scott', 'Dunder Mifflin Corporate', 'michael.scott@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL);

-- CA users
INSERT INTO users (id, role, name, surname, organization, email, email_confirmed, hashed_password, refresh_token, refresh_token_expires_at)
VALUES
    ('3eda5494-d83e-49dc-aa7a-f15375b9f4b5', 'CaUser', 'Jim', 'Halpert', 'Scranton branch', 'jim.halpert@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('4973f67c-a78d-4e7b-bcec-c185a363a015', 'CaUser', 'Pam', 'Beesly', 'Reception department', 'pam.beesly@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('303704ce-e854-4fa4-9fb1-a397d3a46f91', 'CaUser', 'Dwight', 'Schrute', 'Sales department', 'dwight.schrute@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL);

-- EE users
INSERT INTO users (id, role, name, surname, organization, email, email_confirmed, hashed_password, refresh_token, refresh_token_expires_at)
VALUES
    ('6794b78f-f886-41c5-88db-b30dd5175800', 'EeUser', 'Andy', 'Bernard', 'Accounting', 'andy.bernard@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('f5ec2837-bdbd-4fc3-83fb-960bd4e22ad4', 'EeUser', 'Stanley', 'Hudson', 'Warehouse', 'stanley.hudson@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('1115def8-0be6-49d6-9155-0c1f82ed1818', 'EeUser', 'Kevin', 'Malone', 'Finance', 'kevin.malone@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('3fa51d34-4344-4187-8715-6b26889084b2', 'EeUser', 'Angela', 'Martin', 'HR', 'angela.martin@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL),
    ('7edb4bce-6064-461a-8f78-49dbf7c30ecf', 'EeUser', 'Ryan', 'Howard', 'IT department', 'ryan.howard@dundermifflin.com', true, '$2a$10$JO/kNwG1jWv9zuiP5pTkoeH270HGlaMothI4SRpc1n8t6nVScuL5K', '', NULL);

INSERT INTO certificates (serial_number, issued_by, issued_to, not_before, not_after, encoded_value, is_approved, private_key, can_sign, path_len, signing_certificate_serial_number) VALUES ('295066436380222621631274892801870084449', 'CN=dundermifflin.org,O=Dunder Mifflin Corporate,OU=Upper Management,E=issues@dundermifflin.org,C=US', 'CN=dundermifflin.org,O=Dunder Mifflin Corporate,OU=Upper Management,E=issues@dundermifflin.org,C=US', '2005-03-23 23:00:00.000000 +00:00', '2039-12-30 23:00:00.000000 +00:00', 'MIID/DCCAuSgAwIBAgIRYTljns2C5EaihN+mgbr73QAwDQYJKoZIhvcNAQELBQAwgZAxGjAYBgNVBAMMEWR1bmRlcm1pZmZsaW4ub3JnMSEwHwYDVQQKDBhEdW5kZXIgTWlmZmxpbiBDb3Jwb3JhdGUxGTAXBgNVBAsMEFVwcGVyIE1hbmFnZW1lbnQxJzAlBgkqhkiG9w0BCQEWGGlzc3Vlc0BkdW5kZXJtaWZmbGluLm9yZzELMAkGA1UEBhMCVVMwHhcNMDUwMzIzMjMwMDAwWhcNMzkxMjMwMjMwMDAwWjCBkDEaMBgGA1UEAwwRZHVuZGVybWlmZmxpbi5vcmcxITAfBgNVBAoMGER1bmRlciBNaWZmbGluIENvcnBvcmF0ZTEZMBcGA1UECwwQVXBwZXIgTWFuYWdlbWVudDEnMCUGCSqGSIb3DQEJARYYaXNzdWVzQGR1bmRlcm1pZmZsaW4ub3JnMQswCQYDVQQGEwJVUzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAIVxHTGMYmTO0Gm6SxI2cDMvzWEtG5AUgnMUDWV3CrzkbaTh/dqjYbZ41jePf+cBFaBloa7uYZ+v35to/fsDFjcUiaGt41tSw9Jaf0rHc2WebmdfHQmOgmIpIfGnxrFXsELL2H2aMHhXSj3qGJnc1p3/aMtrZgwYM9IkeSflXFy78rFw3BXLkqJehwJddoYaTO75AlB1/IAZ1iqCxuT8oLCgGdjTxaExXRwBGKCJ1q2mYWHW4lWtqv4Z7jFnTVojuqIpEstx/O4FnzypmJ3Bp9LH2BFHCo4OWD0zmWVf0hCVDB1FtH3ZssOngepPN+kVnHWckMGkpwlUyZuX7JQQVKUCAwEAAaNPME0wEgYDVR0TAQH/BAgwBgEB/wIBGTAOBgNVHQ8BAf8EBAMCAYYwJwYDVR0lBCAwHgYIKwYBBQUHAwEGCCsGAQUFBwMCBggrBgEFBQcDAzANBgkqhkiG9w0BAQsFAAOCAQEAbr9XyV2+yjsvvC+kNhvfmQdy7aytYJVCIDhGoWuo9fnzDir5KZA06nPYMx6UfXZ9iGXtc/fVWQjCjTov4hAj15brpTQ7KqmJ5YOE1EWyiaCCC2B1Ir+EXg8otd2J1JuZbamlqL3KQ6st/niQM70mMR1JXV+LhMlH0v5Q2xbkC/BeIx+FqZr+m7ZnPiJMf8sOe72PZIkiGEsfMolNHAxGiMnKlwDt5246gVGEgjq6IoUiaPxJ4Eggsu6oE3QNU96kHGJSPnQw9Kn1PIjv6fsWQ5ERqziyt+3zPukP3XKPzwJggg0fepQ88WenEuYN5N91QZgqmEJBJPGQ7IY48DFl5A==', true, e'-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAhXEdMYxiZM7QabpLEjZwMy/NYS0bkBSCcxQNZXcKvORtpOH9
2qNhtnjWN49/5wEVoGWhru5hn6/fm2j9+wMWNxSJoa3jW1LD0lp/SsdzZZ5uZ18d
CY6CYikh8afGsVewQsvYfZoweFdKPeoYmdzWnf9oy2tmDBgz0iR5J+VcXLvysXDc
FcuSol6HAl12hhpM7vkCUHX8gBnWKoLG5PygsKAZ2NPFoTFdHAEYoInWraZhYdbi
Va2q/hnuMWdNWiO6oikSy3H87gWfPKmYncGn0sfYEUcKjg5YPTOZZV/SEJUMHUW0
fdmyw6eB6k836RWcdZyQwaSnCVTJm5fslBBUpQIDAQABAoIBACiy/cDxfMcbTvSY
RbJJ2UG7aCwl2lzA7KKVsJpMSwGeuBRMeyT2pORHCasLgOqfaY2wwbX9bdgLB6u6
Q5CN+fYVtP6Tbx8Y6LHdOSrBF2CciDbOCyixhvHSctBmKBNJ1/AfhSmV4AesM/Ab
WKKA+RJaZod4e7jfntqOlkdFTLzZgT3Q5rOGqY91YKsrjCxuUdZUww2eUI3stdOW
aLklkTkSAlZSdo6ZgT+ev+3AjX4ONmG/ndSXtpaQdDlCvX/YThzOUbrgjmkm8/cJ
JF30sng4Rz887eP84HIvtWSNiIhtxfJs+bv9ns2yCu/kQxwYlgljVxAWxnxVmmL1
u6jSZmcCgYEA4LkRycGeRCMREi2z7+ODYd4FXKKExxhCh83NQEqNssHM38Kv+FQc
ptFdakBABlupWU66/FaBHTKsXlloCzXtCG577tgW1UhmEPWTgl4LmficmXCmNTp/
IyBUw/gFXa0b/AzPldLRISY3JTi9px80LkW91gU/U7aPmJRqeDJ5pSMCgYEAmAOt
rteWtCd4b655asQ9r5Y+PR8l8NNWFT5vlK8qKxlf5dQ5EFCYghmWUowb5W6L0/sv
Ud02+g0+llJO0h7p9hlDE2amlIULBvsXqDTtMNYre2RMAiwl0QdgsWWN+kIN+BsH
SgppahF2edTK76eRMMnwE+BHF733fvqwWCcvr5cCgYArD4t3C6DWEZodz6AhIYl1
YWPJomKq+90TxL3FygNo83XckqPBg+yRkqDB0VnzXfEPaeSuoazP+XvKAHvNWiH3
caRR2hpV+C+GpGgFnRu//0GSrWFL5c+i3XkgrA/rKVapb5L/dIwaPAZpHXxJ39LR
4w9DSXIfCmZtqFLWgWWJ3QKBgCr/z19HqF9pUs30gm/K9s/2JcDQUMEgZpc3xARA
0CLE2LWAJGwTADC84CKVc0ag0Hiz8pyrgGOW339R5O3WxMNSPgD85l3YjFf7KqPv
5LANxNrwcfejWUcZWSaU3WZOzPjDXHp7G3pfWi34HTdRJOayGHDm3fU/TQTkEIG/
A+QdAoGBAN5x5v41BdtM9DO6RM1gyDFwromzlP0S38VlfGDJdPXEQg9yvD3WHP6M
QIZqS8s3bSDNZmcep6nQ+IYQjpcJIsKSC0Q6vsJM9wQTIJ+luREqtjPQp1ZgWbmS
0LOWQPOdwt4Qh8nx1Iehkk530pX961sumLfuKIEOZvfCmMizZ7OC
-----END RSA PRIVATE KEY-----
', true, 25, null);

