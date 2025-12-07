import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TuiNavigation } from '@taiga-ui/layout';
import { TuiChevron } from '@taiga-ui/kit';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [TuiNavigation, RouterLink, TuiChevron],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.less',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  protected expanded = false;

  generateMenuGroups() {
    return [
      this.createMenuGroup('projects', '@tui.folder', 'Projects', [
        { key: 'My Projects', link: '/projects' },
        { key: 'Team Projects', link: '/projects/team' },
        { key: 'Archived', link: '/projects/archived' },
      ]),
      this.createMenuGroup('tasks', '@tui.check-square', 'Tasks', [
        { key: 'Todo', link: '/tasks/todo' },
        { key: 'In Progress', link: '/tasks/in-progress' },
        { key: 'Completed', link: '/tasks/completed' },
        { key: 'Overdue', link: '/tasks/overdue' },
      ]),
      this.createMenuGroup('calendar', '@tui.calendar', 'Calendar', [
        { key: 'Events', link: '/calendar' },
        { key: 'Schedule', link: '/calendar/schedule' },
        { key: 'Reminders', link: '/calendar/reminders' },
      ]),
    ];
  }

  private createMenuGroup(id: string, icon: string, title: string, items: any[]) {
    return { id, icon, title, items };
  }
}
