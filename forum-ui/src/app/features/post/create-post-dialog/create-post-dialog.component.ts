import { Component, EventEmitter, Output, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Taiga UI Core - только проверенные компоненты
import {
  TuiButton,
  TuiTextfield,
  TuiLabel,
  TuiIcon  // <-- ДОБАВЬТЕ ЭТО
} from '@taiga-ui/core';

// Taiga UI CDK
import { TuiAutoFocus } from '@taiga-ui/cdk';

@Component({
  selector: 'app-create-post-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

    // Taiga UI Core
    TuiButton,
    TuiTextfield,
    TuiLabel,
    TuiIcon,  // <-- ДОБАВЬТЕ ЭТО

    // Taiga UI CDK
    TuiAutoFocus
  ],
  templateUrl: './create-post-dialog.component.html',
  styleUrls: ['./create-post-dialog.component.less']
})
export class CreatePostDialogComponent {
  @Input() open = false;
  @Output() closed = new EventEmitter<void>();
  @Output() postCreated = new EventEmitter<void>();

  post = {
    title: '',
    content: '',
    categoryName: '',
    tagNames: [] as string[]
  };

  categories = [
    { id: '1', name: 'Обсуждение' },
    { id: '2', name: 'Вопрос' },
    { id: '3', name: 'Новости' }
  ];

  tags = [
    { id: '1', name: 'angular' },
    { id: '2', name: 'typescript' },
    { id: '3', name: 'frontend' }
  ];

  isLoading = false;

  createPost(): void {
    if (!this.post.title || !this.post.content || !this.post.categoryName) {
      return;
    }

    this.isLoading = true;

    setTimeout(() => {
      console.log('Creating post:', this.post);
      this.isLoading = false;
      this.postCreated.emit();
      this.closeDialog();
    }, 1000);
  }

  closeDialog(): void {
    this.closed.emit();
    this.resetForm();
  }

  resetForm(): void {
    this.post = {
      title: '',
      content: '',
      categoryName: '',
      tagNames: []
    };
  }
}
