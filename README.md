# Blocked Countries Management API

A .NET 8 Web API for managing blocked countries and validating IP addresses using third-party geolocation APIs. This project uses in-memory data storage and does not rely on a database.

---

## Features

- **Block/Unblock Countries**: Manage a list of blocked countries using their country codes.
- **IP Validation**: Validate IP addresses and check if they belong to blocked countries.
- **Geolocation Integration**: Fetch geolocation details for IP addresses using a third-party API.
- **Audit Logging**: Log attempts to access the API, including blocked attempts.
- **In-Memory Storage**: Uses `ConcurrentDictionary` for storing blocked countries and logs.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A free API key from a geolocation service (e.g., [ipapi.co](https://ipapi.co/) or [IPGeolocation.io](https://ipgeolocation.io/)).

---

## Setup Instructions

### 1. Clone the Repository

### 2. Configure the Geolocation API
Update the `appsettings.json` file with your geolocation API details:

### 3. Restore Dependencies
Run the following command to restore NuGet packages:

### 4. Build and Run the Application

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

---

## API Endpoints

### **IP Lookup**
- **GET** `/api/ip/lookup?ipAddress={ip}`
- Fetch geolocation details for the given IP address. If no IP is provided, the caller's IP is used.

### **Check Block**
- **GET** `/api/ip/check-block`
- Check if the caller's IP address belongs to a blocked country.

### **Manage Blocked Countries**
- **GET** `/api/blocked-countries`
  - Retrieve the list of blocked countries.
- **POST** `/api/blocked-countries/block/{countryCode}`
  - Block a country by its country code.
- **DELETE** `/api/blocked-countries/unblock/{countryCode}`
  - Unblock a country by its country code.

---

## Project Structure

- **Controllers**: API endpoints (`IpLookupController`).
- **Services**: Business logic for geolocation, blocked countries, and audit logging.
- **Models**: Data models for API responses and logs.
- **Background Services**: Periodic cleanup of temporary blocks.

---

## Technologies Used

- **.NET 8**: Latest version of .NET for building modern web APIs.
- **HttpClient**: For making HTTP requests to the geolocation API.
- **In-Memory Storage**: `ConcurrentDictionary` for thread-safe data storage.
- **Swagger**: API documentation and testing.

---

## Future Enhancements

- Add support for rate limiting.
- Implement caching for geolocation responses.
- Add unit tests for controllers and services.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Author

Developed by [YMY9999](https://github.com/YMY9999).
