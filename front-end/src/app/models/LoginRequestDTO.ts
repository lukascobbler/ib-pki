export interface LoginRequestDTO {
  email: string;
  password: string;
  deviceId?: string | null;
}