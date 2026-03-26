import { Component, EventEmitter, Output, Input, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-custom-calendar',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './custom-calendar.component.html',
  styleUrls: ['./custom-calendar.component.css']
})
export class CustomCalendarComponent {
  @Input() selectedDate: Date | null = null;
  @Output() dateSelected = new EventEmitter<Date>();
  
  isOpen = false;
  currentMonth: Date = new Date();
  weeks: (Date | null)[][] = [];
  
  monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'];
  
  dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  ngOnInit() {
    this.generateCalendar();
  }

  toggleCalendar() {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.generateCalendar();
    }
  }

  generateCalendar() {
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();
    
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startingDayOfWeek = firstDay.getDay();
    const daysInMonth = lastDay.getDate();
    
    this.weeks = [];
    let week: (Date | null)[] = [];
    
    // Fill empty days before month starts
    for (let i = 0; i < startingDayOfWeek; i++) {
      week.push(null);
    }
    
    // Fill days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      week.push(new Date(year, month, day));
      
      if (week.length === 7) {
        this.weeks.push(week);
        week = [];
      }
    }
    
    // Fill remaining days
    if (week.length > 0) {
      while (week.length < 7) {
        week.push(null);
      }
      this.weeks.push(week);
    }
  }

  previousMonth() {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() - 1,
      1
    );
    this.generateCalendar();
  }

  nextMonth() {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + 1,
      1
    );
    this.generateCalendar();
  }

  selectDate(date: Date | null) {
    if (date && !this.isPastDate(date)) {
      this.selectedDate = date;
      this.dateSelected.emit(date);
      this.isOpen = false;
    }
  }

  selectToday() {
    const today = new Date();
    this.selectDate(today);
  }

  isSelected(date: Date | null): boolean {
    if (!date || !this.selectedDate) return false;
    return date.toDateString() === this.selectedDate.toDateString();
  }

  isToday(date: Date | null): boolean {
    if (!date) return false;
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  isPastDate(date: Date | null): boolean {
    if (!date) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date < today;
  }

  getFormattedDate(): string {
    if (!this.selectedDate) return 'Select Date';
    const options: Intl.DateTimeFormatOptions = { 
      weekday: 'short', 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    };
    return this.selectedDate.toLocaleDateString('en-US', options);
  }
}
