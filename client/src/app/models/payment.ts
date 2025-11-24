export interface Payment {
  id: string;
  reference?: string;
  clientRequestId?: string;
  amount: number;
  currency: 'USD'|'EUR'|'INR'|'GBP';
  createdAt: Date | string;
  updatedAt?: Date | string;
}
