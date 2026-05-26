import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, Review } from '../../services/api.service';

interface ReviewView {
  text: string;
  author: string;
  rating: number;
  source: string | null;
}

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.css']
})
export class ReviewsComponent implements OnInit {
  private readonly api = inject(ApiService);

  reviews: ReviewView[] = [];
  loading = true;
  error = false;

  ngOnInit(): void {
    this.api.getReviews().subscribe({
      next: (reviews: Review[]) => {
        this.reviews = reviews.map(r => ({
          text: r.comment,
          author: r.customerName,
          rating: r.rating,
          source: r.source
        }));
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }
}
