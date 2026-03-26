import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, withInMemoryScrolling, withViewTransitions, withPreloading, PreloadAllModules } from '@angular/router';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled'
      }),
      withViewTransitions(),
      withPreloading(PreloadAllModules)
    )
  ]
}).catch(err => console.error(err));
