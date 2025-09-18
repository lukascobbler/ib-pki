import {Component} from '@angular/core';
import {FormsModule, NgForm, ReactiveFormsModule} from '@angular/forms';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {DateAdapter, MAT_DATE_FORMATS, MatNativeDateModule} from '@angular/material/core';
import {MatSelect, MatSelectModule} from '@angular/material/select';
import {NgForOf, NgIf} from '@angular/common';
import {CustomDateAdapter} from '../custom-components/custom-date-adapter';
import {CUSTOM_DATE_FORMATS} from '../custom-components/custom-date-formats';
import {CreateCertificate} from '../../../models/CreateCertificate';
import {MatChipsModule} from '@angular/material/chips';
import {KeyUsageValue} from '../../../models/KeyUsageValue';
import {ExtendedKeyUsageValue} from '../../../models/ExtendedKeyUsageValue';
import {CertificatesService} from '../../../services/certificates/certificates.service';

@Component({
  selector: 'app-issue-certificate',
  standalone: true,
  imports: [
    FormsModule,
    MatDatepickerModule,
    MatInputModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    NgForOf,
    NgIf,
    MatChipsModule,
    ReactiveFormsModule
  ],
  providers: [
    {provide: DateAdapter, useClass: CustomDateAdapter},
    {provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS}
  ],
  templateUrl: './issue-certificate.component.html',
  styleUrl: './issue-certificate.component.scss'
})
export class IssueCertificateComponent {
  protected readonly ExtendedKeyUsageValue = ExtendedKeyUsageValue;
  protected readonly KeyUsageValue = KeyUsageValue;
  extensions: { key: string, value: any }[] = [];
  dateNotBefore: Date | null = null;
  dateNotAfter: Date | null = null;
  signingCertificate = ''
  commonName = ''
  organization = ''
  organizationalUnit = ''
  email = ''
  country = ''

  constructor(private certificateService: CertificatesService) {
  }

  onCountryInput(e: Event) {
    const input = e.target as HTMLInputElement;
    const pos = input.selectionStart!;
    this.country = input.value.replace(/[^A-Za-z]/g, '').toUpperCase();
    input.value = this.country;
    input.setSelectionRange(pos, pos);
  }

  allExtensionKeys = [
    {value: 'keyUsage', label: 'Key Usage'},
    {value: 'extendedKeyUsage', label: 'Extended Key Usage'},
    {value: 'subjectAlternativeNames', label: 'Subject Alternative Names'},
    {value: 'issuerAlternativeNames', label: 'Issuer Alternative Names'},
    {value: 'nameConstraints', label: 'Name Constraints'},
    {value: 'basicConstraints', label: 'Basic Constraints'},
    {value: 'certificatePolicies', label: 'Certificate Policies'}
  ];

  getAvailableKeys(currentExt: any) {
    return this.allExtensionKeys.filter(
      key => !this.extensions.some(ext => ext.key === key.value && ext !== currentExt)
    );
  }

  addExtension() {
    if (this.extensions.length < this.allExtensionKeys.length) {
      this.extensions.push({key: '', value: ''});
      const content = document.querySelector('.content');
      if (content) content.scrollTop = content.scrollHeight;
    }
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }

  clearExtensionValue(ext: { key: string; value: any }) {
    if (ext.key === 'subjectAlternativeNames' || ext.key === 'issuerAlternativeNames')
      ext.value = [];
    else if (ext.key === 'nameConstraints')
      ext.value = [[], []];
    else if (ext.key === 'basicConstraints')
      ext.value = {isCA: null, pathLen: null};
    else if (ext.key === 'certificatePolicies')
      ext.value = {policyIdentifier: null, cpsUri: null, userNotice: null};
    else ext.value = '';
    return ext;
  }

  handleKeyDown(event: KeyboardEvent, extValue: any[], prefixRef: MatSelect) {
    if (!['Enter', ','].includes(event.key)) return;
    event.preventDefault();
    const nameInput = event.target as HTMLInputElement;
    const value = nameInput.value?.trim();
    if (!value) return;
    extValue.push({prefix: prefixRef.value, value});
    nameInput.value = '';
  }

  removeChip(extValue: any, chip: any) {
    const index = extValue.indexOf(chip);
    if (index >= 0) extValue.splice(index, 1);
  }

  sanitizePathLen(event: Event, ext: { key: string; value: any }) {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/[^0-9]/g, '');
    if (input.value.startsWith('0')) {
      input.value = input.value.replace(/^0+/, '');
      input.setSelectionRange(0, 0);
    }
    ext.value.pathLen = input.value ? +input.value : null;
  }

  generalNamesToString(list: [{ prefix: string, value: string }]) {
    let result = '';
    list.forEach(item => result += `${item.prefix}:${item.value},`)
    return result.slice(0, -1);
  }

  onSubmit(form: NgForm) {
    if (!form.valid) return;

    const dto: CreateCertificate = {
      signingCertificate: "0",
      commonName: this.commonName,
      organization: this.organization,
      organizationalUnit: this.organizationalUnit,
      email: this.email,
      country: this.country
    }

    if (this.dateNotBefore)
      dto.notBefore = this.dateNotBefore;
    if (this.dateNotAfter)
      dto.notAfter = this.dateNotAfter;

    this.extensions.forEach(extension => {
      console.log(extension);
      if (extension.key === 'keyUsage')
        dto.keyUsage = extension.value.map((v: number) => KeyUsageValue[v]);
      else if (extension.key === 'extendedKeyUsage')
        dto.extendedKeyUsage = extension.value.map((v: number) => ExtendedKeyUsageValue[v]);
      else if (extension.key === 'subjectAlternativeNames' && this.generalNamesToString(extension.value))
        dto.subjectAlternativeNames = {value: this.generalNamesToString(extension.value)};
      else if (extension.key === 'issuerAlternativeNames' && this.generalNamesToString(extension.value))
        dto.issuerAlternativeNames = {value: this.generalNamesToString(extension.value)};
      else if (extension.key === 'nameConstraints' && (this.generalNamesToString(extension.value[0]) || this.generalNamesToString(extension.value[1])))
        dto.nameConstraints = {
          permittedSubtrees: {value: this.generalNamesToString(extension.value[0])},
          excludedSubtrees: {value: this.generalNamesToString(extension.value[1])}
        };
      else if (extension.key === 'basicConstraints')
        dto.basicConstraints = extension.value;
      else if (extension.key === 'certificatePolicy')
        dto.certificatePolicy = extension.value;
    })

    this.certificateService.issueCertificate(dto);
  }
}
