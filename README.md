# ChaosCraft - Chaos Testing Platform

A .NET 8 Blazor Server application for visually creating and running chaos tests on REST APIs using WireMock.

## Features

### Core Functionality
- **Visual Test Creation**: Select API endpoints and chaos test templates through an intuitive UI
- **Real-time Logging**: Monitor test execution with live log updates
- **WireMock Integration**: Automatically generates and runs WireMock configurations
- **AI Explanations**: Get detailed explanations of what each chaos test simulates
- **Export Reports**: Download markdown reports of test results

### Chaos Test Templates
- **Server Error**: Returns HTTP 500 Internal Server Error
- **Slow Response**: Introduces 5-second delays to simulate network latency
- **Not Found**: Returns HTTP 404 for missing resource scenarios
- **Malformed JSON**: Returns invalid JSON to test parsing resilience
- **Random Responses**: Randomly switches between success and error responses
- **Rate Limited**: Returns HTTP 429 Too Many Requests

### API Endpoints
Pre-configured endpoints for testing:
- `/login` - Authentication endpoint
- `/orders` - Order management
- `/checkout` - Payment processing
- `/users` - User management
- `/products` - Product catalog
- `/payments` - Payment processing

### Dashboard Features
- **Template Management**: View and manage chaos test templates
- **Test History**: Browse previous test executions with detailed logs
- **Metrics Panel**: Simulated Grafana-style metrics visualization
- **Real-time Updates**: Live updates of test status and server activity

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Running the Application
1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore` to install dependencies
4. Run `dotnet run` to start the application
5. Open your browser to `https://localhost:7xxx` (port will be displayed in console)

### Using ChaosCraft
1. **Select an Endpoint**: Choose from the dropdown list of API endpoints
2. **Choose a Template**: Click on a chaos test template card
3. **Run Test**: Click "Run Test" to start the WireMock server with your configuration
4. **Monitor**: Watch real-time logs and metrics during test execution
5. **Explain**: Use "Explain Test" to get AI-generated explanations
6. **Export**: Download markdown reports of your test results

## Architecture

### Components
- **ChaosTestRunner**: Main test execution interface
- **MetricsPanel**: Simulated monitoring dashboard
- **Templates Page**: Template management interface
- **Test History**: Historical test data viewer

### Services
- **ChaosTestService**: Core service managing WireMock integration and test execution
- **Real-time Events**: Event-driven updates for logs and test status

### Models
- **ChaosTest**: Represents a test execution instance
- **ChaosTemplate**: Defines chaos test configurations
- **TestReport**: Structured test result data

## Extensibility

The application is designed for easy extension:

### Adding New Templates
1. Add new `ChaosTemplate` objects in `ChaosTestService.InitializeTemplates()`
2. Update the explanation logic in `ExplainTest()` method

### Adding New Endpoints
1. Update the `GetEndpoints()` method in `ChaosTestService`

### Future Integrations
The modular design supports future additions:
- **Terraform/AWS CDK**: Deploy mock environments to cloud
- **Slack/Email Alerts**: Notifications on test failures
- **Database Storage**: Persistent test history
- **Custom Templates**: User-defined chaos scenarios
- **API Integration**: REST API for external tool integration

## Technology Stack
- **.NET 8**: Latest .NET framework
- **Blazor Server**: Real-time web UI framework
- **WireMock.Net**: HTTP service mocking
- **Bootstrap 5**: Responsive UI components
- **SignalR**: Real-time communication (built into Blazor Server)

## License
This project is open source and available under the MIT License.