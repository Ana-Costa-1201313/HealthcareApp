dotnet tool restore
dotnet tool install --global dotnet-ef
dotnet husky install
dotnet restore
dotnet build

cd RecordsBackoffice
npm install nodemon --save-dev
npm install axios