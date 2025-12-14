import { Component, EventEmitter, Output, Input, HostListener, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';

// Taiga UI Core
import {
  TuiButton,
  TuiTextfield,
  TuiLabel,
  TuiIcon,
  TuiLoader
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
    TuiIcon,
    TuiLoader,

    // Taiga UI CDK
    TuiAutoFocus
  ],
  templateUrl: './create-post-dialog.component.html',
  styleUrls: ['./create-post-dialog.component.less']
})
export class CreatePostDialogComponent implements OnDestroy {
  @ViewChild('dialogContent') dialogContent!: ElementRef;

  @Input() set open(value: boolean) {
    this._open = value;
    if (value) {
      this.disableBodyScroll();
    } else {
      this.enableBodyScroll();
    }
  }
  get open(): boolean {
    return this._open;
  }
  private _open = false;

  @Output() closed = new EventEmitter<void>();
  @Output() postCreated = new EventEmitter<void>();

  post = {
    title: '',
    content: '',
    categoryName: '',
    tagNames: [] as string[]
  };

  newTagInput = '';

  categories = [
    { id: '1', name: 'Обсуждение' },
    { id: '2', name: 'Вопрос' },
    { id: '3', name: 'Новости' },
    { id: '4', name: 'Руководство' },
    { id: '5', name: 'Объявление' }
  ];

  availableTags = [
    { id: '1', name: 'angular' },
    { id: '2', name: 'typescript' },
    { id: '3', name: 'frontend' },
    { id: '4', name: 'backend' },
    { id: '5', name: 'javascript' },
    { id: '6', name: 'программирование' },
    { id: '7', name: 'разработка' },
    { id: '8', name: 'web' },
    { id: '9', name: 'css' },
    { id: '10', name: 'html' }
  ];

  isLoading = false;

  constructor(@Inject(DOCUMENT) private document: Document) {}

  // Разрешаем скролл только внутри диалогового окна
  @HostListener('wheel', ['$event'])
  onWheel(event: WheelEvent): void {
    if (this.open) {
      // Проверяем, находится ли событие внутри диалогового окна
      const dialogContent = this.dialogContent?.nativeElement;
      if (dialogContent && !dialogContent.contains(event.target as Node)) {
        // Если событие вне диалога - блокируем
        event.preventDefault();
      }
      // Если событие внутри диалога - разрешаем скролл
    }
  }

  // Блокируем клавиши навигации только для body, не для диалога
  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    if (this.open) {
      const dialogContent = this.dialogContent?.nativeElement;
      const isInsideDialog = dialogContent && dialogContent.contains(event.target as Node);

      // Если нажатие клавиш не внутри диалога - блокируем
      if (!isInsideDialog && (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'PageUp' || event.key === 'PageDown')) {
        event.preventDefault();
      }
      // Если внутри диалога - разрешаем навигацию (например, в textarea)
    }
  }

  // Обработка нажатия Escape для закрытия диалога
  @HostListener('document:keydown', ['$event'])
  onDocumentKeyDown(event: KeyboardEvent): void {
    if (this.open && event.key === 'Escape') {
      event.preventDefault();
      this.closeDialog();
    }
  }

  // Блокируем скролл body с сохранением позиции
  private disableBodyScroll(): void {
    const scrollY = window.scrollY;
    const body = this.document.body;

    body.style.position = 'fixed';
    body.style.top = `-${scrollY}px`;
    body.style.left = '0';
    body.style.right = '0';
    body.style.overflow = 'hidden';
    body.style.width = '100%';

    // Сохраняем позицию скролла в data-атрибут для восстановления
    body.setAttribute('data-scroll-y', scrollY.toString());
  }

  // Восстанавливаем скролл body
  private enableBodyScroll(): void {
    const body = this.document.body;
    const scrollY = body.getAttribute('data-scroll-y');

    body.style.position = '';
    body.style.top = '';
    body.style.left = '';
    body.style.right = '';
    body.style.overflow = '';
    body.style.width = '';

    // Восстанавливаем позицию скролла
    if (scrollY) {
      window.scrollTo(0, parseInt(scrollY, 10));
      body.removeAttribute('data-scroll-y');
    }
  }

  ngOnDestroy(): void {
    this.enableBodyScroll();
  }

  // Добавить пользовательский тег
  addCustomTag(event?: Event): void {
    if (event) {
      event.preventDefault();
    }

    const tag = this.newTagInput.trim();
    if (tag && !this.post.tagNames.includes(tag)) {
      this.post.tagNames.push(tag);
      this.newTagInput = '';
    }
  }

  // Удалить тег
  removeTag(tagName: string): void {
    const index = this.post.tagNames.indexOf(tagName);
    if (index > -1) {
      this.post.tagNames.splice(index, 1);
    }
  }

  // Переключить тег из доступных
  toggleTag(tagName: string): void {
    if (this.post.tagNames.includes(tagName)) {
      this.removeTag(tagName);
    } else {
      this.post.tagNames.push(tagName);
    }
  }

  createPost(): void {
    if (!this.post.title || !this.post.content || !this.post.categoryName) {
      return;
    }

    this.isLoading = true;

    // Имитация запроса к API
    setTimeout(() => {
      console.log('Создан пост:', {
        ...this.post,
        tagNames: [...this.post.tagNames]
      });

      this.isLoading = false;
      this.postCreated.emit();
      this.resetForm();
    }, 1500);
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
    this.newTagInput = '';
  }
}
