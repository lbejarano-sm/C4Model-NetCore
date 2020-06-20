using Structurizr;
using Structurizr.Api;
using System.Linq;

namespace C4Model
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }

        static void Banking()
        {
            const long workspaceId = 0;
            const string apiKey = "";
            const string apiSecret = "";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("Banking", "Banking - C4 Model");
            Model model = workspace.Model;

            SoftwareSystem internetBankingSystem = model.AddSoftwareSystem("Internet Banking", "Permite a los clientes consultar información de sus cuentas y realizar operaciones.");
            SoftwareSystem mainframeBankingSystem = model.AddSoftwareSystem("Mainframe Banking", "Almacena información del core bancario.");
            SoftwareSystem mobileAppSystem = model.AddSoftwareSystem("Mobile App", "Permite a los clientes consultar información de sus cuentas y realizar operaciones.");
            SoftwareSystem emailSystem = model.AddSoftwareSystem("SendGrid", "Servicio de envío de notificaciones por email.");

            Person cliente = model.AddPerson("Cliente", "Cliente del banco.");
            Person cajero = model.AddPerson("Cajero", "Empleado del banco.");

            mainframeBankingSystem.AddTags("Mainframe");
            mobileAppSystem.AddTags("Mobile App");
            emailSystem.AddTags("SendGrid");

            cliente.Uses(internetBankingSystem, "Realiza consultas y operaciones bancarias.");
            cliente.Uses(mobileAppSystem, "Realiza consultas y operaciones bancarias.");
            cajero.Uses(mainframeBankingSystem, "Usa");

            internetBankingSystem.Uses(mainframeBankingSystem, "Usa");
            internetBankingSystem.Uses(emailSystem, "Envía notificaciones de email");
            mobileAppSystem.Uses(internetBankingSystem, "Usa");

            emailSystem.Delivers(cliente, "Envía notificaciones de email", "SendGrid");

            ViewSet viewSet = workspace.Views;

            // 1. Diagrama de Contexto
            SystemContextView contextView = viewSet.CreateSystemContextView(internetBankingSystem, "Contexto", "Diagrama de contexto - Banking");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();
            //contextView.EnableAutomaticLayout();

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.Person) { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Mobile App") { Background = "#29c732", Color = "#ffffff", Shape = Shape.MobileDevicePortrait });
            styles.Add(new ElementStyle("Mainframe") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("SendGrid") { Background = "#a5cdff", Color = "#ffffff", Shape = Shape.RoundedBox });

            // 2. Diagrama de Contenedores
            Container webApplication = internetBankingSystem.AddContainer("Aplicación Web", "Permite a los clientes consultar información de sus cuentas y realizar operaciones.", "ReactJS, nginx port 80");
            Container restApi = internetBankingSystem.AddContainer("RESTful API", "Permite a los clientes consultar información de sus cuentas y realizar operaciones.", "Net Core, nginx port 80");
            Container worker = internetBankingSystem.AddContainer("Worker", "Manejador del bus de mensajes.", "Net Core");
            Container database = internetBankingSystem.AddContainer("Base de Datos", "Repositorio de información bancaria.", "Oracle 12c port 1521");
            Container messageBus = internetBankingSystem.AddContainer("Bus de Mensajes", "Transporte de eventos del dominio.", "RabbitMQ");

            webApplication.AddTags("WebApp");
            restApi.AddTags("API");
            worker.AddTags("Worker");
            database.AddTags("Database");
            messageBus.AddTags("MessageBus");

            cliente.Uses(webApplication, "Usa", "https 443");
            webApplication.Uses(restApi, "Usa", "https 443");
            worker.Uses(restApi, "Usa", "https 443");
            worker.Uses(messageBus, "Usa");
            worker.Uses(mainframeBankingSystem, "Usa");
            restApi.Uses(database, "Usa", "jdbc 1521");
            restApi.Uses(messageBus, "Usa");
            restApi.Uses(emailSystem, "Usa", "https 443");
            mobileAppSystem.Uses(restApi, "Usa");

            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9Ii0xMS41IC0xMC4yMzE3NCAyMyAyMC40NjM0OCI+CiAgPHRpdGxlPlJlYWN0IExvZ288L3RpdGxlPgogIDxjaXJjbGUgY3g9IjAiIGN5PSIwIiByPSIyLjA1IiBmaWxsPSIjNjFkYWZiIi8+CiAgPGcgc3Ryb2tlPSIjNjFkYWZiIiBzdHJva2Utd2lkdGg9IjEiIGZpbGw9Im5vbmUiPgogICAgPGVsbGlwc2Ugcng9IjExIiByeT0iNC4yIi8+CiAgICA8ZWxsaXBzZSByeD0iMTEiIHJ5PSI0LjIiIHRyYW5zZm9ybT0icm90YXRlKDYwKSIvPgogICAgPGVsbGlwc2Ugcng9IjExIiByeT0iNC4yIiB0cmFuc2Zvcm09InJvdGF0ZSgxMjApIi8+CiAgPC9nPgo8L3N2Zz4K" });
            styles.Add(new ElementStyle("API") { Background = "#929000", Color = "#ffffff", Shape = Shape.RoundedBox, Icon = "https://dotnet.microsoft.com/static/images/redesign/downloads-dot-net-core.svg?v=U_8I9gzFF2Cqi5zUNx-kHJuou_BWNurkhN_kSm3mCmo" });
            styles.Add(new ElementStyle("Worker") { Icon = "https://dotnet.microsoft.com/static/images/redesign/downloads-dot-net-core.svg?v=U_8I9gzFF2Cqi5zUNx-kHJuou_BWNurkhN_kSm3mCmo" });
            styles.Add(new ElementStyle("Database") { Background = "#ff0000", Color = "#ffffff", Shape = Shape.Cylinder, Icon = "https://4.bp.blogspot.com/-5JVtZBLlouA/V2LhWdrafHI/AAAAAAAADeU/_3bo_QH1WGApGAl-U8RkrFzHjdH6ryMoQCLcB/s200/12cdb.png" });
            styles.Add(new ElementStyle("MessageBus") { Width = 850, Background = "#fd8208", Color = "#ffffff", Shape = Shape.Pipe, Icon = "https://www.rabbitmq.com/img/RabbitMQ-logo.svg" });

            ContainerView containerView = viewSet.CreateContainerView(internetBankingSystem, "Contenedor", "Diagrama de contenedores - Banking");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();
            //containerView.EnableAutomaticLayout();

            // 3. Diagrama de Componentes
            Component transactionController = restApi.AddComponent("Transactions Controller", "Allows users to perform transactions.", "Spring Boot REST Controller");
            Component signinController = restApi.AddComponent("SignIn Controller", "Allows users to sign in to the Internet Banking System.", "Spring Boot REST Controller");
            Component accountsSummaryController = restApi.AddComponent("Accounts Controller", "Provides customers with an summary of their bank accounts.", "Spring Boot REST Controller");
            Component securityComponent = restApi.AddComponent("Security Component", "Provides functionality related to signing in, changing passwords, etc.", "Spring Bean");
            Component mainframeBankingSystemFacade = restApi.AddComponent("Mainframe Banking System Facade", "A facade onto the mainframe banking system.", "Spring Bean");

            restApi.Components.Where(c => "Spring Boot REST Controller".Equals(c.Technology)).ToList().ForEach(c => webApplication.Uses(c, "Uses", "HTTPS"));
            signinController.Uses(securityComponent, "Uses");
            accountsSummaryController.Uses(mainframeBankingSystemFacade, "Uses");
            securityComponent.Uses(database, "Reads from and writes to", "JDBC");
            mainframeBankingSystemFacade.Uses(mainframeBankingSystem, "Uses", "XML/HTTPS");

            ComponentView componentViewForRestApi = viewSet.CreateComponentView(restApi, "Components", "The components diagram for the REST API");
            componentViewForRestApi.PaperSize = PaperSize.A4_Landscape;
            componentViewForRestApi.AddAllContainers();
            componentViewForRestApi.AddAllComponents();
            componentViewForRestApi.Add(cliente);
            componentViewForRestApi.Add(mainframeBankingSystem);
            //componentViewForRestApi.EnableAutomaticLayout();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}