import { Routes } from '@angular/router';
import { PaymentList } from './components/Payment/PaymentList/payments';
import { PaymentForm } from './components/Payment/Payment-form/payment-form';

export const routes: Routes = [
  { path: '', component: PaymentList },
  {
    path: 'payments/add',
    component: PaymentForm
  },
  { path: 'payments/edit/:id', component: PaymentForm }
];
