[Перейти к русской версии](#ru) | [Go to English version](#en)

<a name="ru"></a>
# PhotoEquipmentStore — мой учебный проект для магазина фотооборудования (дипломная работа)

## Краткое описание
Это мой учебный десктоп-проект для управления ассортиментом, заказами, клиентами и генерации отчётов/чеков. Проект создан для демонстрации моих практических навыков разработки: многослойная архитектура, DTO, сервисы, интеграция с базой данных MySQL и генерация PDF/отчётов. UI реализован с помощью Avalonia.

## Назначение
- Показать мой опыт в разработке многослойного .NET-приложения.
- Учебный проект для дипломной работы: в нём реализованы бизнес-логика, слой данных/инфраструктуры и графический интерфейс.
- Управление товарами, заказами, клиентами и генерация чеков и отчётов.

## Ключевые возможности
- Управление товарами (CRUD).
- Управление клиентами (CRUD).
- Создание и обработка заказов.
- Генерация PDF-чека и отчётов (см. `PhotoEquipmentStore.Application/Reports`).
- Импорт данных (CSV/Excel — см. `ImportService`).
- Проверка и тестирование подключения к базе данных MySQL.
- Авторизация и управление пользователями.
- UI: Avalonia — кроссплатформенный десктоп-интерфейс.
- Архитектура: Desktop UI ↔ Application (контракты/сервисы) ↔ Infrastructure (доступ к данным, MySQL) ↔ Domain (сущности).

## Структура репозитория (основные папки)
- `PhotoEquipmentStore.sln` — решение .NET.
- `PhotoEquipmentStore.Desktop/` — десктоп-приложение (UI на Avalonia).
  - `App.axaml`, `App.axaml.cs`, `Program.cs` — точки входа и конфигурация UI.
  - `ViewModels/`, `Views/` — MVVM-слой UI.
  - `Assets/` — изображения, иконки, стили.
  - `Controls/`, `Converters/`, `Behaviors/` — вспомогательные компоненты UI.
- `PhotoEquipmentStore.Application/` — бизнес-логика, DTO, сервисы.
  - `DTO/` — переносимые объекты данных.
  - `Interfaces/` — контракты сервисов.
  - `Services/` — реализации сервисов (`ProductsService.cs`, `OrderService.cs`, `ImportService.cs`, `ReceiptPdfService.cs` и т.д.).
  - `Reports/` — генерация отчётов (`ReportBuilder.cs`, `StyleFactory.cs`).
  - `Helpers/` — утилиты (`ImageCompressor.cs`, `PasswordHasher.cs`).
- `PhotoEquipmentStore.Domain/` — бизнес-сущности и перечисления.
- `PhotoEquipmentStore.Infrastructure/` — уровень доступа к данным, реализация подключения к MySQL, скрипты.

## Требования / Зависимости
- .NET SDK (целевой фреймворк указан в `.csproj`).
- Avalonia для UI (указано в зависимостях проекта Desktop).
- MySQL — в проекте используется MySQL как основная СУБД; настройки подключения находятся в `PhotoEquipmentStore.Infrastructure/Connection` или в сервисе подключения (`ConnectionServise.cs`).

## Настройка подключения к базе данных
- Конфигурация подключения и скрипты для MySQL находятся в `PhotoEquipmentStore.Infrastructure/Connection`.
- Для локальной разработки можно развернуть MySQL контейнер или локальный экземпляр MySQL и настроить строку подключения в соответствующих конфигурационных файлах/переменных окружения.

## Ограничения
- Проект учебный/дипломный — предназначен для демонстрации моих навыков, а не для production.
- Возможно, не все проверки безопасности и оптимизации выполнены для боевого использования.

---

<a name="en"></a>
# PhotoEquipmentStore — my educational photo equipment store desktop application (diploma project)

## Short description
This is my educational desktop application for managing a photo equipment store — inventory, customers, orders, and report/receipt generation. I created this project to demonstrate practical skills: layered architecture, DTOs, services, integration with MySQL, and PDF/report generation. UI is implemented using Avalonia.

## Purpose
- To showcase my experience in building a layered .NET application.
- Diploma/learning project that implements business logic, data/infrastructure layer and a desktop UI.
- Manage products, customers, orders, and generate receipts and reports.

## Key features
- Product management (CRUD).
- Client management (CRUD).
- Order creation and processing.
- PDF receipt and report generation (see `PhotoEquipmentStore.Application/Reports`).
- Data import (CSV/Excel — see `ImportService`).
- MySQL connection checking and utilities.
- Authorization and user management.
- UI: Avalonia — cross-platform desktop interface.
- Architecture: Desktop UI ↔ Application (contracts/services) ↔ Infrastructure (data access, MySQL) ↔ Domain (entities).

## Repository structure (main folders)
- `PhotoEquipmentStore.sln` — .NET solution.
- `PhotoEquipmentStore.Desktop/` — desktop application (UI on Avalonia).
  - `App.axaml`, `App.axaml.cs`, `Program.cs` — entry points and UI configuration.
  - `ViewModels/`, `Views/` — MVVM UI layer.
  - `Assets/` — images, icons, styles.
  - `Controls/`, `Converters/`, `Behaviors/` — UI helper components.
- `PhotoEquipmentStore.Application/` — business logic, DTOs, services.
  - `DTO/` — Data Transfer Objects.
  - `Interfaces/` — service contracts.
  - `Services/` — service implementations (`ProductsService.cs`, `OrderService.cs`, `ImportService.cs`, `ReceiptPdfService.cs`, etc.).
  - `Reports/` — report generation (`ReportBuilder.cs`, `StyleFactory.cs`).
  - `Helpers/` — utilities (`ImageCompressor.cs`, `PasswordHasher.cs`).
- `PhotoEquipmentStore.Domain/` — domain entities and enums.
- `PhotoEquipmentStore.Infrastructure/` — data access layer, MySQL connection helpers, scripts.

## Requirements / Dependencies
- .NET SDK (target framework is specified in `.csproj`).
- Avalonia for UI (declared in Desktop project dependencies).
- MySQL — used as the primary database; connection settings are in `PhotoEquipmentStore.Infrastructure/Connection` or in the connection service (`ConnectionServise.cs`).

## Database configuration
- Check `PhotoEquipmentStore.Infrastructure/Connection` for MySQL connection configuration and any provided scripts.
- For development, use a local MySQL instance or a containerized MySQL and configure the connection string in the appropriate configuration files or environment variables.

## Limitations and notes
- Educational/diploma project — intended to demonstrate my skills only.
- May lack production-level security checks and optimizations.
