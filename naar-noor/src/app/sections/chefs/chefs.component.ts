import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, Chef } from '../../services/api.service';

interface ChefView {
  name: string;
  role: string;
  image: string;
  bio: string;
  specialty: string;
}

@Component({
  selector: 'app-chefs',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './chefs.component.html',
  styleUrls: ['./chefs.component.css']
})
export class ChefsComponent implements OnInit {
  private readonly api = inject(ApiService);

  chefs: ChefView[] = [];
  loading = true;
  error = false;

  ngOnInit(): void {
    this.api.getChefs().subscribe({
      next: (chefs: Chef[]) => {
        this.chefs = chefs.map(chef => ({
          name: chef.name,
          role: chef.title,
          image: chef.imageUrl || 'assets/chefs/chef-placeholder.jpg',
          bio: chef.bio,
          specialty: chef.specialty
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
