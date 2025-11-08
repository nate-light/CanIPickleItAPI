# Can I Pickle It API

This ASP.NET Core Web API determines whether items can be pickled by consulting the OpenAI ChatGPT API.

## Setup

### 1. Configure OpenAI API Key

You need to provide your OpenAI API key in one of the following ways:

#### Option A: Update appsettings.json
Replace `"your-openai-api-key-here"` in `appsettings.json` or `appsettings.Development.json` with your actual OpenAI API key:

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-actual-api-key-here"
  }
}
```

#### Option B: Use Environment Variables (Recommended for production)
Set the environment variable:
```bash
set OPENAI__APIKEY=sk-your-actual-api-key-here
```

Or in PowerShell:
```powershell
$env:OPENAI__APIKEY="sk-your-actual-api-key-here"
```

#### Option C: User Secrets (Recommended for development)
```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-actual-api-key-here"
```

### 2. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or the port specified in your launch settings).

## API Endpoints

### POST /api/pickle/can-pickle

Check if an item can be pickled using a JSON request body.

**Request Body:**
```json
{
  "item": "cucumber"
}
```

**Response:**
```json
{
  "canPickle": true,
  "reason": "Yes, cucumbers can be pickled. They are one of the most common vegetables used for pickling due to their firm texture and ability to absorb flavors well."
}
```

**Example using curl:**
```bash
curl -X POST "https://localhost:7000/api/pickle/can-pickle" \
     -H "Content-Type: application/json" \
     -d '{"item": "cucumber"}'
```

### GET /api/pickle/can-pickle?item={item}

Check if an item can be pickled using a query parameter.

**Example:**
```
GET https://localhost:7000/api/pickle/can-pickle?item=cucumber
```

**Response:**
```json
{
  "canPickle": true,
  "reason": "Yes, cucumbers can be pickled. They are one of the most common vegetables used for pickling due to their firm texture and ability to absorb flavors well."
}
```

## Swagger Documentation

When running in development mode, you can access the Swagger UI at:
```
https://localhost:7000/swagger
```

## Examples

### Items that can typically be pickled:
- Cucumbers
- Onions
- Carrots
- Beets
- Cabbage
- Eggs
- Various fruits

### Items that typically cannot be pickled:
- Leafy greens (lettuce, spinach)
- Very soft fruits
- Dairy products
- Meat (without special processing)

## Error Handling

The API includes comprehensive error handling:
- Invalid or empty item names return a 400 Bad Request
- OpenAI API errors are logged and return a default "cannot determine" response
- All errors are logged for debugging purposes

## Configuration

The application uses the standard ASP.NET Core configuration system. You can override settings using:
- Environment variables
- User secrets (development)
- Command line arguments
- Configuration files (appsettings.json)

## Dependencies

- .NET 8.0
- System.Text.Json (for JSON serialization)
- Swashbuckle.AspNetCore (for API documentation)
- Built-in HttpClient (for OpenAI API calls)