import { Component, EventEmitter, Output, Input, HostListener, OnDestroy, ElementRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, lastValueFrom } from 'rxjs';

// Taiga UI Core
import {
  TuiButton,
  TuiTextfield,
  TuiIcon,
  TuiLoader,
} from '@taiga-ui/core';

// Taiga UI CDK
import { TuiAutoFocus } from '@taiga-ui/cdk';

// API импорты
import { ApiConfiguration } from '../../../api/api-configuration';
import { CreatePostCommand } from '../../../api/models/create-post-command';
import { CategoryModel } from '../../../api/models/category-model';
import { TagModel } from '../../../api/models/tag-model';
import { CreateCategoryCommand } from '../../../api/models/create-category-command';

// Интерфейс для создаваемого поста
export interface NewPostData {
  title: string;
  content: string;
  categoryName: string;
  tagNames: string[];
}

@Component({
  selector: 'app-create-post-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

    // Taiga UI Core
    TuiButton,
    TuiTextfield,
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
      this.loadInitialData();
    } else {
      this.enableBodyScroll();
    }
  }
  get open(): boolean {
    return this._open;
  }
  private _open = false;

  @Output() closed = new EventEmitter<void>();
  @Output() postCreated = new EventEmitter<NewPostData>();

  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfiguration);

  post = {
    title: '',
    content: '',
    categoryName: '',
    tagNames: [] as string[]
  };

  // Новые поля для создания категории
  newCategoryInput = '';
  isCreatingCategory = false;

  newTagInput = '';
  errorMessage = '';
  successMessage = '';

  categories: CategoryModel[] = [];
  availableTags: TagModel[] = [];

  isLoading = false;
  isCreatingPost = false;

  constructor(@Inject(DOCUMENT) private document: Document) {}

  // Загрузка начальных данных при открытии диалога
  private async loadInitialData(): Promise<void> {
    console.log('Loading initial data for create post dialog...');

    this.isLoading = true;
    this.errorMessage = '';

    try {
      await Promise.all([
        this.loadCategories(),
        this.loadTags()
      ]);

      console.log('Initial data loaded successfully');
    } catch (error) {
      console.error('Error loading initial data:', error);
      this.errorMessage = 'Не удалось загрузить данные для создания поста';
    } finally {
      this.isLoading = false;
    }
  }

  private async loadCategories(): Promise<void> {
    console.log('Loading categories for create post dialog...');

    const token = localStorage.getItem('accessToken');
    if (!token) {
      console.error('No token found for loading categories');
      return;
    }

    try {
      const response = await lastValueFrom(
        this.http.get<CategoryModel[]>(
          `${this.apiConfig.rootUrl}/api/forum/v1/Category/GetCategories`,
          { headers: { Authorization: `Bearer ${token}` } }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error loading categories:', error);
            console.error('Error status:', error.status);
            console.error('Error message:', error.message);
            throw error;
          })
        )
      );

      this.categories = response || [];
      console.log(`Loaded ${this.categories.length} categories`);

    } catch (error) {
      console.error('Failed to load categories:', error);
      this.categories = [];
    }
  }

  private async loadTags(): Promise<void> {
    console.log('Loading tags for create post dialog...');

    const token = localStorage.getItem('accessToken');
    if (!token) {
      console.error('No token found for loading tags');
      return;
    }

    try {
      const response = await lastValueFrom(
        this.http.get<TagModel[]>(
          `${this.apiConfig.rootUrl}/api/forum/v1/Tag/GetTags`,
          { headers: { Authorization: `Bearer ${token}` } }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error loading tags:', error);
            console.error('Error status:', error.status);
            console.error('Error message:', error.message);
            throw error;
          })
        )
      );

      this.availableTags = response || [];
      console.log(`Loaded ${this.availableTags.length} tags`);

    } catch (error) {
      console.error('Failed to load tags:', error);
      this.availableTags = [];
    }
  }

  // Выбор категории из доступных
  selectCategory(categoryName: string): void {
    this.post.categoryName = categoryName;
  }

  // Очистка выбранной категории
  clearCategory(): void {
    this.post.categoryName = '';
  }

  // Добавление пользовательской категории
  async addCustomCategory(event?: Event): Promise<void> {
    if (event) {
      event.preventDefault();
    }

    const categoryName = this.newCategoryInput.trim();
    if (!categoryName) {
      this.errorMessage = 'Введите название категории';
      return;
    }

    const token = localStorage.getItem('accessToken');
    if (!token) {
      this.errorMessage = 'Требуется авторизация для создания категории';
      return;
    }

    console.log('Creating new category:', categoryName);

    this.isCreatingCategory = true;
    this.errorMessage = '';

    const createCategoryCommand: CreateCategoryCommand = {
      name: categoryName,
      description: undefined
    };

    console.log('CreateCategoryCommand:', createCategoryCommand);

    try {
      // Отправляем запрос на создание категории
      const response = await lastValueFrom(
        this.http.post(
          `${this.apiConfig.rootUrl}/api/forum/v1/Category/CreateCategory`,
          createCategoryCommand,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error creating category:', error);
            console.error('Error status:', error.status);
            console.error('Error message:', error.message);
            console.error('Error response:', error.error);

            if (error.status === 401) {
              this.errorMessage = 'Требуется авторизация. Возможно, ваш токен истек';
              localStorage.removeItem('accessToken');
              localStorage.removeItem('refreshToken');
            } else if (error.status === 400) {
              this.errorMessage = 'Ошибка валидации: ' + (error.error?.message || 'Проверьте введенные данные');
            } else if (error.status === 403) {
              this.errorMessage = 'Недостаточно прав для создания категории';
            } else {
              this.errorMessage = 'Ошибка создания категории: ' + (error.message || 'Попробуйте позже');
            }
            throw error;
          })
        )
      );

      console.log('Category created successfully:', response);

      // Обновляем список категорий
      await this.loadCategories();

      // Устанавливаем новую категорию как выбранную
      this.post.categoryName = categoryName;

      // Очищаем поле ввода
      this.newCategoryInput = '';

      this.successMessage = 'Категория успешно создана и выбрана!';

    } catch (error) {
      console.error('Failed to create category:', error);
    } finally {
      this.isCreatingCategory = false;
    }
  }

  // Разрешаем скролл только внутри диалогового окна
  @HostListener('wheel', ['$event'])
  onWheel(event: WheelEvent): void {
    if (this.open) {
      const dialogContent = this.dialogContent?.nativeElement;
      if (dialogContent && !dialogContent.contains(event.target as Node)) {
        event.preventDefault();
      }
    }
  }

  // Блокируем клавиши навигации только для body, не для диалога
  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    if (this.open) {
      const dialogContent = this.dialogContent?.nativeElement;
      const isInsideDialog = dialogContent && dialogContent.contains(event.target as Node);

      if (!isInsideDialog && (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'PageUp' || event.key === 'PageDown')) {
        event.preventDefault();
      }
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

  async createPost(): Promise<void> {
    if (!this.post.title || !this.post.content || !this.post.categoryName) {
      this.errorMessage = 'Пожалуйста, заполните все обязательные поля';
      return;
    }

    const token = localStorage.getItem('accessToken');
    if (!token) {
      this.errorMessage = 'Требуется авторизация для создания поста';
      console.error('No token found for creating post');
      return;
    }

    console.log('Creating post with data:', this.post);

    this.isCreatingPost = true;
    this.errorMessage = '';
    this.successMessage = '';

    const createPostCommand: CreatePostCommand = {
      title: this.post.title,
      content: this.post.content,
      categoryName: this.post.categoryName,
      tagNames: this.post.tagNames.length > 0 ? this.post.tagNames : undefined
    };

    console.log('CreatePostCommand:', createPostCommand);

    try {
      // Отправляем запрос на создание поста
      const response = await lastValueFrom(
        this.http.post(
          `${this.apiConfig.rootUrl}/api/forum/v1/Post/CreatePost`,
          createPostCommand,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error creating post:', error);
            console.error('Error status:', error.status);
            console.error('Error message:', error.message);
            console.error('Error response:', error.error);

            if (error.status === 401) {
              this.errorMessage = 'Требуется авторизация. Возможно, ваш токен истек';
              localStorage.removeItem('accessToken');
              localStorage.removeItem('refreshToken');
            } else if (error.status === 400) {
              this.errorMessage = 'Ошибка валидации: ' + (error.error?.message || 'Проверьте введенные данные');
            } else if (error.status === 403) {
              this.errorMessage = 'Недостаточно прав для создания поста';
            } else {
              this.errorMessage = 'Ошибка создания поста: ' + (error.message || 'Попробуйте позже');
            }
            throw error;
          })
        )
      );

      console.log('Post created successfully:', response);

      this.successMessage = 'Пост успешно создан!';

      // Ждем немного, чтобы пользователь увидел сообщение об успехе
      setTimeout(() => {
        // Эмитируем событие с данными поста
        this.postCreated.emit({
          title: this.post.title,
          content: this.post.content,
          categoryName: this.post.categoryName,
          tagNames: [...this.post.tagNames]
        });

        this.isCreatingPost = false;
        this.resetForm();
      }, 1500);

    } catch (error) {
      console.error('Failed to create post:', error);
      this.isCreatingPost = false;
    }
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
    this.newCategoryInput = '';
    this.errorMessage = '';
    this.successMessage = '';
    this.isCreatingCategory = false;
  }
}
