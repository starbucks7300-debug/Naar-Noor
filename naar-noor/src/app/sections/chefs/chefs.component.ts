import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, Chef } from '../../services/api.service';
import { RevealDirective } from '../../directives/scroll-reveal.directive';

const CHEF_IMAGES = [
  'assets/chefs/chef-arjun.jpg',
  'assets/chefs/chef-maya.jpg',
];

interface ChefView {
  name: string;
  role: string;
  image: string;
  bio: string;
  specialty: string;
  initial: string;
}

@Component({
  selector: 'app-chefs',
  standalone: true,
  imports: [CommonModule, RevealDirective],
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
        this.chefs = chefs.map((chef, i) => ({
          name: chef.name,
          role: chef.title,
          image: chef.imageUrl || CHEF_IMAGES[i % CHEF_IMAGES.length],
          bio: chef.bio,
          specialty: chef.specialty,
          initial: chef.name.charAt(0)
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
    return i * 120;
  }
}
