# Build stage
FROM node:20-alpine as builder

WORKDIR /app

# Копируем файлы зависимостей
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем исходный код
COPY . .

# Собираем приложение
RUN npm run build

# Production stage
FROM nginx:alpine

# Копируем собранные файлы из build stage
COPY --from=builder /app/dist/inventory-track/browser /usr/share/nginx/html

# Копируем конфигурацию nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Открываем порт
EXPOSE 4200

# Запускаем nginx
CMD ["nginx", "-g", "daemon off;"]
