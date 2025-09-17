import {ExtendedKeyUsageValue} from './ExtendedKeyUsageValue';
import {KeyUsageValue} from './KeyUsageValue';

export interface CreateCertificateDTO {
  signingCertificateId: string;
  commonName: string;
  organization: string;
  organizationalUnit: string;
  email: string;
  country: string;
  notBefore?: string;
  notAfter?: string;
  keyUsage?: KeyUsageValue[];
  extendedKeyUsage?: ExtendedKeyUsageValue[];
  subjectAlternativeNames?: ListOfNames;
  issuerAlternativeNames?: ListOfNames;
  nameConstraints?: NamesConstraintsValue;
  basicConstraints?: BasicConstraintsValue;
  certificatePolicy?: CertificatePolicy;
}

export interface ListOfNames {
  value: string;
}

export interface NamesConstraintsValue {
  permittedSubtrees?: ListOfNames;
  excludedSubtrees?: ListOfNames;
}

export interface BasicConstraintsValue {
  isCa: boolean;
  pathLenConstraint?: number;
}

export interface CertificatePolicy {
  policyIdentifier: string;
  cpsUri: string;
  userNotice: string;
}
