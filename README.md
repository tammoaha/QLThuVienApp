# QLThuVienApp

Ung dung WinForms quan ly muon tra sach thu vien, viet bang C# .NET Framework 4.8 va SQL Server.

## Chuc nang chinh

- Dang nhap thu thu.
- Quan ly sach, the loai, sinh vien, thu thu.
- Lap phieu muon sach va tra sach.
- Thong ke sach con lai va phieu muon chua tra.

## Yeu cau moi truong

- Windows.
- Visual Studio 2019/2022.
- .NET Framework 4.8 Developer Pack.
- SQL Server hoac SQL Server Express.
- SQL Server Management Studio de chay script database.

## Restore database

1. Mo SQL Server Management Studio.
2. Ket noi vao SQL Server.
   - Neu dung SQL Server mac dinh: `localhost` hoac `.`
   - Neu dung SQL Server Express: `.\SQLEXPRESS`
3. Mo file `QLThuVien_SQLServer.sql`.
4. Bam `Execute` de tao database `QLThuVien`, cac bang, khoa ngoai, stored procedure va du lieu mau.
5. Refresh `Databases` va kiem tra co database `QLThuVien`.

File `QLThuVien_MySQL.sql` chi la ban cu dung MySQL, khong dung cho yeu cau hien tai cua de bai SQL Server.

## Cau hinh ket noi

Mo `App.config` va kiem tra connection string.

Neu dung SQL Server mac dinh:

```xml
<add name="QLThuVien"
     connectionString="Data Source=localhost;Initial Catalog=QLThuVien;Integrated Security=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

Neu dung SQL Server Express:

```xml
<add name="QLThuVien"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

Neu dang nhap bang tai khoan SQL Server, vi du `sa`:

```xml
<add name="QLThuVien"
     connectionString="Data Source=localhost;Initial Catalog=QLThuVien;User ID=sa;Password=mat_khau_cua_ban;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

## Chay ung dung

1. Mo `QLThuVienApp.csproj` bang Visual Studio.
2. Chon `Build > Rebuild Solution`.
3. Bam `F5` de chay.
4. Dang nhap bang tai khoan mau:

```text
Ten dang nhap: admin
Mat khau: 123456
```

Tai khoan tro ly thu thu de kiem tra phan quyen:

```text
Ten dang nhap: troly
Mat khau: 123456
Quyen: TroLy
```

Quyen `TroLy` khong duoc tao, them, sua tai khoan thu thu.

## Loi thuong gap

### Keyword not supported: 'port'

Ung dung dang doc config MySQL cu co `Port=3306`. Hay sua `App.config` sang SQL Server, tat app dang chay, sau do build lai.

### Cannot open database 'QLThuVien'

Database chua duoc tao hoac app dang tro sai SQL Server instance. Hay chay `QLThuVien_SQLServer.sql` trong SSMS va kiem tra `Data Source` trong `App.config`.

### Login failed for user

Tai khoan Windows hien tai chua co quyen vao database, hoac connection string sai kieu dang nhap. Neu can cap quyen cho Windows user hien tai, co the chay trong SSMS:

```sql
USE QLThuVien;
GO

CREATE USER [JUMP-WINDOWS\VICTUS] FOR LOGIN [JUMP-WINDOWS\VICTUS];
GO

ALTER ROLE db_owner ADD MEMBER [JUMP-WINDOWS\VICTUS];
GO
```

Neu user da ton tai, SQL Server se bao loi da co user; khi do chi can chay lenh `ALTER ROLE`.
