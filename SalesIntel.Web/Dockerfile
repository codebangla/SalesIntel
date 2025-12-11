# Stage 1: Build frontend
FROM node:18-alpine AS build
WORKDIR /app

COPY . .
RUN npm install
RUN npm run build

# Stage 2: Serve static files
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
