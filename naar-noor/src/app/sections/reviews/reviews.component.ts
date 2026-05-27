import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, Review } from '../../services/api.service';
import { RevealDirective } from '../../directives/scroll-reveal.directive';

interface ReviewView {
  text: string;
  author: string;
  initial: string;
  rating: number;
  source: string | null;
  stars: number[];
}

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule, RevealDirective],
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
          initial: r.customerName.charAt(0).toUpperCase(),
          rating: r.rating,
          stars: Array.from({ length: r.rating }, (_, i) => i),
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

  revealDelay(i: number): number {
    return i * 100;
  }

  sourceIcon(source: string | null): string {
    if (!source) return '';
    const map: Record<string, string> = {
      'Google': 'logos:google-icon',
      'TripAdvisor': 'simple-icons:tripadvisor',
      'Direct': 'solar:star-bold'
    };
    return map[source] || 'solar:star-bold';
  }
}
