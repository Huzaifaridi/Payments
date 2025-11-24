import { Component, signal } from '@angular/core';
import { PaymentService } from '../../../services/payment.service';
import { Payment } from '../../../models/payment';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payments.html',
  styleUrls: ['./payments.css'],
})
export class PaymentList {

    loading = false;
  title = 'Home Page';
   payments = signal<Payment[]>([]);
  constructor(private svc: PaymentService) {}

  ngOnInit(): void {
    this.getPayments();
  }

  getPayments() {
    this.svc.getPayments().subscribe(res => {
      const formatted = res.map(item => ({
        ...item,
        createdAt: new Date(item.createdAt) // force conversion
      }));
      this.payments.set(formatted);
    });
  }
  delete(p: Payment) {
    if (!confirm(`Are you sure you want to delete this payment ${p.reference}?`)) return;
    this.svc.deletePayment(p.id).subscribe(() => this.getPayments());
  }
}
