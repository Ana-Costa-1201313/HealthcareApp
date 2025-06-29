dotnet tool restore
dotnet tool install --global dotnet-ef
cd Backoffice
dotnet ef database drop
rmdir /s /q Migrations 
dotnet ef migrations add InitialCreate 
dotnet ef migrations add AddPhoneNumberConverter
dotnet ef migrations add EmailConverter
dotnet ef database update