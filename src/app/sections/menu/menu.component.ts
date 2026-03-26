import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {
  menuItems = [
    {
      name: 'Himalayan Momo Selection',
      price: '£14',
      description: 'Steamed dumplings filled with spiced ground meat or vegetables, served with roasted tomato and sesame achar.'
    },
    {
      name: 'Flame-Grilled Yeti Platter',
      price: '£32',
      description: 'An assortment of overnight marinated lamb chops, chicken tikka, and smoked pork belly, grilled over charcoal.'
    },
    {
      name: 'Traditional Thukpa Bowl',
      price: '£18',
      description: 'Hearty hand-pulled noodle soup enriched with slow-cooked bone broth, seasonal greens, and chili crisp.'
    },
    {
      name: 'Yeti Special Curry',
      price: '£24',
      description: 'Tender goat meat slow-braised in a rich, dark gravy of black cardamom, fenugreek, and secret mountain spices.'
    },
    {
      name: 'Saffron Dessert Delight',
      price: '£11',
      description: 'Warm reduced milk pudding infused with mountain saffron, topped with crushed pistachios and edible gold leaf.'
    }
  ];
}
