# Hướng dẫn chạy Database Migration

Tài liệu này giúp bạn chạy database migration cho dự án **NDTCore.Identity.API** bằng cả **CMD** và **PowerShell**, phù hợp cho các môi trường Windows khác nhau. Mỗi script được đóng gói để đảm bảo tính an toàn và idempotent (chạy nhiều lần không gây lỗi).

## 1. Sử dụng Batch Scripts (Windows CMD)

Batch scripts thích hợp khi bạn đang làm việc trong môi trường Windows thuần hoặc khi chạy qua CI/CD dùng Windows agents.

```bat
:: Mở Command Prompt
cd NDTCore.Identity.API

:: Chạy migration chính
Scripts\migrate.bat

:: Hoặc chạy utilities
Scripts\migration-utils.bat
```

---

## 2. Sử dụng PowerShell Scripts

PowerShell scripts phù hợp khi bạn muốn chạy được trên nhiều môi trường hơn (Windows, Linux qua PowerShell 7) hoặc cần logging chi tiết hơn.

```powershell
# Mở PowerShell
cd NDTCore.Identity.API

# Chạy migration chính
./Scripts/migrate.ps1

# Hoặc chạy utilities
./Scripts/migration-utils.ps1
```