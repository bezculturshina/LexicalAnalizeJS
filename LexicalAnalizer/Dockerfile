# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем все исходные файлы проекта
COPY error.cs .
COPY lex.cs .
COPY loc.cs .
COPY main.cs .
COPY scan.cs .
COPY text.cs .
COPY LexicalAnalizer.csproj .

# Восстанавливаем зависимости
RUN dotnet restore LexicalAnalizer.csproj

# Сборка проекта и публикация
RUN dotnet publish LexicalAnalizer.csproj -c Release -o out

# Этап выполнения
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/out .

# Копируем входной файл (если есть)
COPY example1.txt .

# Команда по умолчанию
CMD ["dotnet", "LexicalAnalizer.dll", "example1.txt"]
