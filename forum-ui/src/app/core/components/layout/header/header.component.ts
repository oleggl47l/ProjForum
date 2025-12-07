import {
  ChangeDetectionStrategy,
  Component,
} from '@angular/core';
import { TuiButton, TuiIcon } from '@taiga-ui/core';
import { TuiNavigation } from '@taiga-ui/layout';

@Component({
  standalone: true,
  selector: 'app-header',
  imports: [
    TuiButton,
    TuiNavigation,
    TuiIcon,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.less',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {

}
