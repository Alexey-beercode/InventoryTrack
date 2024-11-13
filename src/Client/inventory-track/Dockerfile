# Используем базовый образ Node.js
FROM node:20-alpine as builder

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /app

# Копируем package.json и package-lock.json (если он есть)
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем остальную часть клиентского кода
COPY . .

# Собираем Angular приложение
RUN npm run build --prod

# Устанавливаем сервер Angular CLI
FROM node:20-alpine

WORKDIR /app

# Копируем файлы сборки из предыдущего этапа
COPY --from=builder /app/dist/inventory-track /app

# Устанавливаем Angular CLI
RUN npm install -g @angular/cli

# Запускаем Angular сервер
CMD ["ng", "serve"]
