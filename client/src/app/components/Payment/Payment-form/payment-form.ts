import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { v4 as uuidv4 } from 'uuid';
import { PaymentService } from '../../../services/payment.service';
import { Payment } from '../../../models/payment';

import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-payment-form',
  templateUrl: './payment-form.html',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  styleUrls: ['./payment-form.css']
})
export class PaymentForm implements OnInit {

  paymentForm!: FormGroup;
  id!: string;
  isEditMode = false;
  currencies = ['USD', 'EUR', 'INR', 'GBP'];

  constructor(
    private fb: FormBuilder,
    private service: PaymentService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.id = this.route.snapshot.paramMap.get('id') ?? '';

    this.paymentForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(1)]],
      currency: ['', Validators.required],
      clientRequestId: ['']
    });

    if (this.id) {
      this.isEditMode = true;
      this.loadPayment();
    }
  }

  loadPayment() {
    this.service.getPaymentById(this.id).subscribe(payment => {
      this.paymentForm.patchValue(payment);
    });
  }

  onSubmit() {
    if (this.paymentForm.invalid) return;

    const payment: Payment = this.paymentForm.value;
    if (!this.isEditMode) {
        payment.id = uuidv4();
        payment.clientRequestId = uuidv4();
        
      this.service.createPayment(payment).subscribe(() => {
        alert("Payment Created Successfully!");
        this.router.navigate(['/']);
      });
    } else {
      this.service.updatePayment(this.id, payment).subscribe(() => {
        alert("Payment Updated Successfully!");
        this.router.navigate(['/']);
      });
    }
  }

  deletePayment() {
    if (confirm('Are you sure you want to delete this payment?')) {
      this.service.deletePayment(this.id).subscribe(() => {
        alert("Payment Deleted.");
        this.router.navigate(['/']);
      });
    }
  }

}
