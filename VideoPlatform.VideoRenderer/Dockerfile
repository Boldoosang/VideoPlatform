# Use official Node.js LTS image based on Debian
FROM node:20-slim


# Set working directory
WORKDIR /usr/src/app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Install necessary packages including system Chromium and its dependencies
RUN apt-get update && apt-get install -y \
    chromium \
    libnss3 \
    libatk-bridge2.0-0 \
    libxss1 \
    libasound2 \
    libxshmfence1 \
    libgtk-3-0 \
    libgbm1 \
    libdrm2 \
    libxcomposite1 \
    libxrandr2 \
    libxcursor1 \
    libxdamage1 \
    libxfixes3 \
    libatk1.0-0 \
    libcups2 \
    libdbus-1-3 \
    libgdk-pixbuf2.0-0 \
    libpangocairo-1.0-0 \
    libpango-1.0-0 \
    fonts-liberation \
    libappindicator3-1 \
    xdg-utils \
    wget \
    ca-certificates \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*


# Set Remotion to use system Chromium and skip bundled download
ENV REMOTION_CHROMIUM_EXECUTABLE=/usr/bin/chromium
ENV REMOTION_SKIP_CHROMIUM_DOWNLOAD=true

# Copy rest of the app source
COPY . .

# Expose port
EXPOSE 8080

# Start the server
CMD ["node", "app.js"]
