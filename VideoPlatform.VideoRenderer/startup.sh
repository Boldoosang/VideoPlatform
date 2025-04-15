#!/bin/bash

# Install dependencies
apt-get update -y
apt-get install -y \
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
    ca-certificates

# Then run your app
npm start
