# 💳 Simple Microservice Payment System

## 🧩 شرح پروژه

این پروژه یک سامانه‌ی ساده برای **مدیریت تراکنش‌های پرداخت در معماری میکروسرویس** است که شامل سه سرویس اصلی می‌باشد:

1. **Payment Service** – مدیریت تراکنش‌ها، ذخیره‌سازی در پایگاه داده PostgreSQL و انتشار رویدادها در RabbitMQ  
2. **Gateway Service** – شبیه‌سازی فرایند پرداخت، اعتبارسنجی توکن و ارسال نتیجه به Payment Service  
3. **Notification Service** – دریافت رویدادهای پرداخت (PaymentProcessedEvent) از RabbitMQ و ثبت یا نمایش اعلان‌ها

ارتباط بین سرویس‌ها به‌صورت **Event-Driven (مبتنی بر پیام)** از طریق RabbitMQ انجام می‌شود.

---

## ⚙️ معماری استفاده‌شده

معماری مبتنی بر **DDD (Domain-Driven Design)** و **Microservices** با تفکیک به لایه‌های زیر است:

```
Payment.Api
Payment.Application
Payment.Domain
Payment.Infrastructure

Gateway.Api
Gateway.Application
Gateway.Domain
Gateway.Infrastructure

Notification.Api
Notification.Application
Notification.Domain
Notification.Infrastructure
```

هر سرویس API مستقل اجرا می‌شود و از **Dependency Injection**، **Entity Framework Core (Code-First)**،  
و **RabbitMQ** برای برقراری ارتباط بین سرویس‌ها استفاده می‌کند.

---

## 🧰 تکنولوژی‌های استفاده‌شده

- **.NET 9 (ASP.NET Core Web API)**
- **PostgreSQL**
- **Entity Framework Core (Code First)**
- **RabbitMQ (Event Bus)**
- **Quartz.NET** برای اجرای jobهای زمان‌بندی‌شده در PaymentService  
- **Docker Compose** برای استقرار کل سیستم
- **Swagger** برای مستندسازی
- **Console Logging** برای مشاهده‌ی رخدادها

---

## 🚀 نحوه راه‌اندازی

### 1️⃣ راه‌اندازی RabbitMQ بصورت لوکال (اختیاری)
اگر Docker نصب دارید:
```bash
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
```
سپس در مرورگر باز کنید:  
👉 [http://localhost:15672](http://localhost:15672)  
نام کاربری و رمز عبور: `guest / guest`

---

### 2️⃣ اجرای پروژه‌ها با Docker Compose

در پوشه‌ی اصلی پروژه، دستور زیر را اجرا کنید:

```bash
docker compose -f docker-compose.override.yml down -v
docker compose -f docker-compose.override.yml up --build
```

پس از اجرا، سرویس‌ها در پورت‌های زیر در دسترس هستند:

| سرویس | آدرس |
|--------|--------|
| Gateway | http://localhost:5002/swagger/index.html |
| Payment | http://localhost:5001/swagger/index.html |
| Notification | http://localhost:5003/swagger/index.html |
| RabbitMQ UI | http://localhost:15672 |

---

### 3️⃣ تست ارتباط سرویس‌ها

1. به کمک swagger ابتدا از طریق Payment API یک توکن جدید ایجاد کنید (`/api/payment/get-token`)  
2. سپس در Gateway API توکن به دست آمده را وارد کنید یا مستقیما gatewayUrl حاصل از get-token را در تب جدید مرورگر وارد کنید:  
   `GET /api/gateway/pay/{token}`  
3. نتیجه پرداخت در RabbitMQ منتشر می‌شود و NotificationService آن را دریافت می‌کند.  
4. در کنسول `Notification.Api` باید پیام مشابه زیر دیده شود:

```
📩 PaymentProcessedEvent received!
Token: ...
Amount: 250000
Status: Success
```

5. برای مشاهده رکوردهای جدول Transaction در cmd مراحل زیر را طی کنید:
```bash
docker exec -it postgres bash
psql -U admin -d paymentdb
```
سپس دستور SELECT * FROM "Transactions"; را وارد کنید.
---
## 🧠 تصمیمات طراحی

- استفاده از **RabbitMQ Fanout Exchange** برای جداسازی Publisher و Subscriberها.  
- نگه‌داشتن ساختار **لایه‌ای و DDD** برای افزایش قابلیت نگهداری و تست‌پذیری.  
- استفاده از **Quartz.NET** جهت زمان‌بندی عملیات Expire در PaymentService.  
- تمام سرویس‌ها قابلیت اجرا به صورت مستقل یا در Docker Compose را دارند.  

---

## ⚔️ چالش‌های پیاده‌سازی

- مدیریت ارتباطات بین سرویس‌ها در محیط لوکال (localhost vs rabbitmq در Docker)
- هماهنگی بین ساختار Exchange و Queue در RabbitMQ
- مدیریت خطاها و rollback در تراکنش‌ها
- اطمینان از اجرای هم‌زمان Quartz jobs و رویدادهای RabbitMQ

---

## 💡 پیشنهادات بهبود

- **افزودن Health Checks و Web UI:**  
  با استفاده از پکیج `AspNetCore.Diagnostics.HealthChecks` می‌توان وضعیت سلامت هر سرویس و وابستگی‌های آن (RabbitMQ، PostgreSQL، HTTP Dependencies) را پایش کرد و یک صفحه‌ی وب برای نمایش وضعیت کلی سیستم ساخت.

- **افزودن لایه‌ی Logging مرکزی** با استفاده از Elastic Stack یا Seq برای مشاهده‌ی متمرکز لاگ‌ها.

- **استفاده از MassTransit یا CAP Framework** برای مدیریت Event Bus در پروژه‌های بزرگ‌تر.

- **افزودن JWT Authentication** در Gateway برای امنیت بیشتر APIها.

---

## 📦 ساختار پروژه در Docker Compose

```yaml
services:
  payment.api:
    ports: ["5001:8080"]
  gateway.api:
    ports: ["5002:8080"]
  notification.api:
    ports: ["5003:8080"]
  postgres:
    image: postgres:15
    ports: ["5432:5432"]
  rabbitmq:
    image: rabbitmq:3.12-management
    ports: ["5672:5672", "15672:15672"]
```

---

## 🏁 نتیجه

این پروژه نمونه‌ای از معماری **Microservices + DDD + Event-Driven** است  
که ارتباط سرویس‌ها از طریق RabbitMQ انجام می‌شود و به سادگی می‌توان سرویس‌های جدید مانند **Notification، Reporting، یا Analytics** را به آن افزود.
