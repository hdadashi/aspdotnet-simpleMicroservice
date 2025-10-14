# 💳 Simple Microservices Payment System

## 🧭 1. شرح پروژه

این پروژه یک **سیستم ساده‌ی میکروسرویسی پرداخت (Payment System)** است که شامل دو سرویس مستقل است:

- **Payment Service**  
  مسئول ایجاد تراکنش، صدور توکن، اعتبارسنجی، بروزرسانی وضعیت پرداخت و انتشار پیام‌ها در RabbitMQ است.

- **Gateway Service**  
  نقش "درگاه پرداخت" را بازی می‌کند. توکن پرداخت را از Payment دریافت کرده، عملیات پرداخت را شبیه‌سازی می‌کند (با احتمال ۸۰٪ موفق و ۲۰٪ ناموفق)، نتیجه را به PaymentService اطلاع می‌دهد و کاربر را به آدرس merchant هدایت می‌کند.

---

### 🏗️ معماری کلی

پروژه بر اساس **معماری میکروسرویس + DDD (Domain Driven Design)** طراحی شده و هر سرویس دارای ۴ لایه‌ی مستقل است:

```
ServiceName/
├── Api              → Web layer (Controllers, Middlewares)
├── Application      → Business logic, Services, Jobs
├── Domain           → Entities, Enums, ValueObjects
└── Infrastructure   → Database, Messaging, Persistence
```

---

### ⚙️ تکنولوژی‌های استفاده‌شده

| لایه | ابزار / تکنولوژی |
|------|------------------|
| Backend Framework | **.NET 9 (ASP.NET Core Web API)** |
| ORM | **Entity Framework Core (Code First)** |
| Database | **PostgreSQL** |
| Messaging | **RabbitMQ** |
| Scheduler | **Quartz.NET** (برای expire کردن تراکنش‌های pending هر ۳۰ ثانیه) |
| Validation | **FluentValidation** |
| Documentation | **Swagger (Swashbuckle)** |
| Design Pattern | **Repository + Service + DI (Dependency Injection)** |
| Architecture | **DDD + Clean Architecture + Microservices** |

---

## ⚡ 2. نحوه راه‌اندازی و Migration

### 🐇 راه‌اندازی RabbitMQ
اگر Docker نصب داری، کافیست RabbitMQ را با این دستور بالا بیاوری:

```bash
docker run -d --hostname rabbitmq-host --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

- Dashboard: [http://localhost:15672](http://localhost:15672)  
- Username: `guest`  
- Password: `guest`

---

### 🗄️ راه‌اندازی PostgreSQL (اختیاری در Docker)

```bash
docker run --name postgres -e POSTGRES_PASSWORD=12345 -p 5432:5432 -d postgres
```

---

### 🧩 اجرای Migration (برای PaymentService)

در مسیر `PaymentService/Payment.Infrastructure` یا ریشه‌ی سرویس:
```bash
dotnet ef database update --project Payment.Infrastructure --startup-project Payment.Api
```

---

### ▶️ اجرای سرویس‌ها

#### 1️⃣ PaymentService
```bash
cd PaymentService/Payment.Api
dotnet run
```

پیش‌فرض پورت: `https://localhost:5001`

#### 2️⃣ GatewayService
```bash
cd GatewayService/Gateway.Api
dotnet run
```

پیش‌فرض پورت: `https://localhost:5002`

---

### 🔍 تست سریع

1. در Postman بزن:
   ```
   POST https://localhost:5001/api/payment/get-token
   ```
   خروجی شامل `token` و `gatewayUrl` است.

2. با آدرس برگشتی `gatewayUrl` برو:
   ```
   GET https://localhost:5002/api/gateway/pay/{token}
   ```
   پرداخت با احتمال ۸۰٪ موفقیت شبیه‌سازی می‌شود.

3. Gateway نتیجه را به `/api/payment/update-status` می‌فرستد و PaymentService پیام `PaymentProcessedEvent` را در RabbitMQ منتشر می‌کند.

---

## 🧱 3. معماری و تصمیمات طراحی

### 💡 چرا از این ساختار استفاده شد؟

- **میکروسرویس‌ها**: جداسازی منطق و استقلال توسعه/استقرار هر بخش (Payment, Gateway, Notification و …)  
- **DDD + Clean Architecture**: جداسازی دقیق لایه‌ها (Domain از Application و Infrastructure مستقل است).  
- **CQRS-ready design**: ساختار آماده برای افزودن Query/Command با MediatR.  
- **RabbitMQ**: برای decouple شدن سرویس‌ها و انتقال پیام بین آن‌ها.  
- **Quartz.NET**: برای زمان‌بندی وظایف background بدون وابستگی به cron یا task scheduler خارجی.

---

### ⚙️ چالش‌های پیاده‌سازی

- هماهنگی بین سرویس‌ها در زمان تست (به‌خصوص با HTTPS و HttpClient)
- مدیریت خطاها و timeout در ارتباط Gateway → Payment
- اعتبارسنجی توکن‌ها و انقضا با job زمان‌بندی‌شده
- پیکربندی درست DI بین پروژه‌های جدا در یک Solution
- اطمینان از idempotent بودن عملیات update-status

---

### 🚀 اگر زمان بیشتری داشتم...

1. افزودن **NotificationService** برای ارسال پیام پس از پرداخت موفق (مصرف‌کننده RabbitMQ)  
2. افزودن **Authentication / JWT** برای ایمن کردن endpointها  
3. نوشتن **Integration Tests** بین Gateway و Payment  
4. افزودن **Polly** برای Retry / Circuit Breaker در HttpClient  
5. مستندسازی OpenAPI بهتر با نمونه درخواست‌ها و پاسخ‌ها  
6. Docker Compose کامل برای اجرای تمام سرویس‌ها + RabbitMQ + PostgreSQL

---

### ✨ نتیجه

این پروژه نمونه‌ای ساده ولی استاندارد از پیاده‌سازی معماری **Microservice + DDD** با .NET است  
که پایه‌ای عالی برای ساخت سیستم‌های پرداخت یا تراکنش‌های مستقل در مقیاس بزرگ‌تر محسوب می‌شود.

---

📬 **نویسنده:** Hossein  
📅 **تاریخ:** 2025-10  
🧱 **نسخه:** v1.0.0
