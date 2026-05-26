import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomCalendarComponent } from '../../components/custom-calendar/custom-calendar.component';
import { CustomDropdownComponent } from '../../components/custom-dropdown/custom-dropdown.component';
import { ApiService } from '../../services/api.service';

interface ReservationForm {
  fullName: string;
  email: string;
  phone: string;
  date: Date | null;
  time: string;
  guests: string;
  specialRequests: string;
}

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [CommonModule, FormsModule, CustomCalendarComponent, CustomDropdownComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.css']
})
export class ReservationComponent {
  private readonly api = inject(ApiService);

  reservation: ReservationForm = {
    fullName: '',
    email: '',
    phone: '',
    date: null,
    time: '18:00',
    guests: '2 People',
    specialRequests: ''
  };

  timeSlots = ['18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30'];
  guestOptions = ['1 Person', '2 People', '3 People', '4 People', '5 People', '6 People', '7 People', '8 People'];

  submitting = false;
  submitted = false;
  submitError = '';
  confirmedName = '';

  onDateSelected(date: Date): void {
    this.reservation.date = date;
  }

  onTimeSelected(time: string): void {
    this.reservation.time = time;
  }

  onGuestsSelected(guests: string): void {
    this.reservation.guests = guests;
  }

  private parsePartySize(guests: string): number {
    const match = guests.match(/\d+/);
    return match ? parseInt(match[0], 10) : 2;
  }

  private formatDate(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  onSubmit(): void {
    this.submitError = '';

    if (!this.reservation.fullName.trim()) {
      this.submitError = 'Please enter your full name.';
      return;
    }
    if (!this.reservation.email.trim()) {
      this.submitError = 'Please enter your email address.';
      return;
    }
    if (!this.reservation.phone.trim()) {
      this.submitError = 'Please enter your phone number.';
      return;
    }
    if (!this.reservation.date) {
      this.submitError = 'Please select a date.';
      return;
    }

    this.submitting = true;

    this.api.createReservation({
      customerName: this.reservation.fullName.trim(),
      email: this.reservation.email.trim(),
      phoneNumber: this.reservation.phone.trim(),
      reservationDate: this.formatDate(this.reservation.date),
      reservationTime: this.reservation.time,
      partySize: this.parsePartySize(this.reservation.guests),
      specialRequests: this.reservation.specialRequests.trim() || undefined
    }).subscribe({
      next: () => {
        this.submitting = false;
        this.submitted = true;
        this.confirmedName = this.reservation.fullName;
        this.reservation = {
          fullName: '', email: '', phone: '',
          date: null, time: '18:00',
          guests: '2 People', specialRequests: ''
        };
      },
      error: (err) => {
        this.submitting = false;
        if (err.status === 400 && err.error?.errors) {
          const messages = Object.values(err.error.errors).flat() as string[];
          this.submitError = messages[0] || 'Please check your details and try again.';
        } else {
          this.submitError = 'Unable to submit your reservation. Please call us or try again.';
        }
      }
    });
  }

  bookAnother(): void {
    this.submitted = false;
    this.confirmedName = '';
  }
}
