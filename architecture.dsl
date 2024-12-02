workspace {

    model {
        user = person "User" {
            description "A user who interacts with the frontend."
        }

        externalDataSource = softwareSystem "External Data Source"{
            description "Source of data, e.g., sensors, external APIs."
            tags "externalDataSource"
        }
        
       
        system = softwareSystem "Custom Web App" {
            description "The custom web application for energy monitoring and management."
            
            frontendContainer = container "Frontend" {
                description "User interface for interacting with the system."
                tags "frontendContainer"
                
                authComponent = component "Authentication" {
                    description "Handles user login and authentication."
                    tags "authComponent"
                }
                
                settingsComponent = component "Settings/Configuration" {
                    description "Allows users to configure account and system settings."
                    tags "settingsComponent"
                }
                
                profileComponent = component "User Profile" {
                    description "Allows users to manage personal data and account settings."
                    tags "profileComponent"
                }
                
                dataVizComponent = component "Data Visualization" {
                    description "Displays real-time and historical data visualizations."
                    tags "dataVizComponent"
                }
                
                dashboard = component "Dashboards" {
                    description "Displays key metrics and data summaries."
                    tags "dashboard"
                }

                notificationComponent = component "Error & Notification" {
                    description "Shows notifications and error messages to users."
                    tags "notificationComponent"
                }

                dataControlComponent = component "Data Control" {
                    description "Enables users to control data collection and processing."
                    tags "dataControlComponent"
                }

                
                
            }
    
            apiGateway = container "API Gateway" {
                description "Manages API requests and routes them to appropriate services."
                tags "apiGateway"
                
                routingComponent = component "Routing Engine" {
                    description "Handles routing of incoming API requests to the appropriate backend services."
                    tags "routingComponent"
                }
                
                securityComponent = component "Security Layer" {
                    description "Ensures that only authorized requests pass through, handling authentication and authorization checks."
                    tags "securityComponent"
                }
                
                loggingComponent = component "API Logging" {
                    description "Logs API requests and responses for monitoring and troubleshooting."
                    tags "loggingComponent"
                }
            }
    
            authContainer = container "Authentication And Configuration Service" {
                description "Handles user authentication and authorization."
                tags "authContainer"
                
                
                authServiceAPI = component "REST API" {
                    description "Provides an API for handling user authentication requests, MQTT Broker configurations"
                    tags "authServiceAPI"
                }
                
                 authServiceRepository = component "Repository" {
                    description "Stores and retrieves user data and Modbus configurations"
                    tags "authServiceRepository"
                }
            }

            dataCollector = container "Data Collector Service" {
                description "Collects data from various sources and prepares it for storage and analysis."
                tags "dataCollector"
                
                dataSourceConnectors = component "Data Source Connectors" {
                    description "Handles connections to various data sources (Modbus) and performs initial data processing."
                    tags "dataSourceConnectors"
                }
                
                 temporarilyRepository = component "Data Repository" {
                    description "Stores processed data temporarily before it is sent to the Communication Interface."
                    tags "temporarilyRepository"
                }
            
                dataPublisher = component "Data Producer" {
                    description "Formats data and sends it to RabbitMQ for further processing and storage."
                    tags "dataPublisher"
                }
                
                messageBroker = component "Message Broker" {
                    description "Buffering layer between Data Collector and Data Storage services."
                    tags "messageBroker"
                }
            }
            
            errorHandling = container "Error Handling and Logging" {
                    description "Captures and logs errors during data collection and data publishing."
                    tags "errorHandling"
                    /*
                    "Monitors and logs errors from all components; manages retry and fallback strategies."
                    */
                    logDatabase = component "Log Error Data Base" {
                        description "Stores events and errors of all system components."
                        tags "logDatabase"
                    }
                    
                    dataAnalysisVisualization = component "Dashboards " {
                        description "Creates visual dashboards to monitor logs and errors."
                        tags "dataAnalysisVisualization"
                    }
                    
                }

            dataStorage = container "Data Storage Service" {
                description "Stores the collected data."
                tags "dataStorage"
                
                timeSeriesDB = component "Repository of transformed data" {
                    description "Stores time-series data."
                    tags "timeSeriesDB"
                }
				
				consumerFomattedData = component "Consumer Fomatted Data" {
                    description "Reads the transformed data to store it"
                    tags "consumerFomattedData"
                }

                apiInterface = component "REST API" {
                    description "Provides an API for querying data from data base."
                    tags "apiInterface"
                }
            }
    
            dataProcessing = container "Data Processing Service" {
                description "Processes and analyzes the collected data."
                tags "dataProcessing"
                timeSeriesDB2 = component "Aggregate data repository" {
                    description "Data is prepared for reports or in custom formats."
                    tags "timeSeriesDB2"
                }
                rawDataProcessor = component "Raw Data Processor" {
                    description "Accesses and processes data from"
                    tags "rawDataProcessor"
                }
                restAPI = component "REST API" {
                    description "Receives task requests."
                    tags "restAPI"
                }
                
                workloadQueue = component "Workload Queue" {
                    description "The message queue acts as a buffer and handles the asynchronous processing of the request."
                    tags "workloadQueue"
                }
            }
        }

        user -> frontendContainer "Uses"
        frontendContainer -> apiGateway "Makes API calls to"
        
        
        dataSourceConnectors -> externalDataSource "Recover data from"
        
        apiGateway -> authServiceAPI "Forwards authentication and authorization requests"
        
        
        authServiceAPI -> authServiceRepository "Performs CRUD operations on user data"
        
        
        // Relationships frontendContainer
        dashboard -> apiGateway "Requests dashboard data"
        authComponent -> apiGateway "User Logs In/Registers"
        settingsComponent -> apiGateway "Requests and updates application settings"
        profileComponent -> apiGateway "Requests user profile data"
        dataVizComponent -> apiGateway "Requests data for visualization"
        dataControlComponent -> apiGateway "Sends data control commands"
        NotificationComponent  -> apiGateway "Requests user notifications"
        
        
        
        
        loggingComponent -> routingComponent "Logs all interctions"
        loggingComponent -> securityComponent "Logs all interctions"
        frontendContainer -> securityComponent "Sends a request"
        frontendContainer -> routingComponent "Sends a request"
        securityComponent -> authServiceAPI "Forwards request"
        routingComponent -> apiInterface "Forwards request"
        routingComponent -> restAPI "Forwards request"
        
        
        // Relationships dataCollector
        
        dataSourceConnectors -> authServiceRepository "Reads Modbus configurations"
        dataSourceConnectors -> temporarilyRepository "Forwards processed data to be stored"
        temporarilyRepository -> dataPublisher "Provides data to"
        dataPublisher -> messageBroker "Sends formatted data to"
        dataSourceConnectorsLogDatabase = dataSourceConnectors -> logDatabase "Logs and errors to"
        dataPublisherLogDatabase = dataPublisher -> logDatabase "Logs and errors to"
        
        
                
        // Relationships dataStorage
        apiGatewaydataStorage = apiGateway -> apiInterface "Forwards requests"
        consumerFomattedDataMessageBroker = consumerFomattedData -> messageBroker "Reads data from"
		consumerFomattedData -> timeSeriesDB "Writes data to"
		relConsumerFomattedDataLogDatabase = consumerFomattedData -> logDatabase "Logs and errors to"
		relApiInterfaceLogDatabase = apiInterface -> logDatabase "Logs and errors to"
        timeSeriesDB -> apiInterface "Exposes data for querying"
        
        
		//Relationships dataProcessing
        apiGatewayRestAPI = apiGateway -> restAPI "Forwards requests"
        rawDataProcessor -> timeSeriesDB2 "Writes data to"
        rawDataProcessor -> workloadQueue "Reads the request from the queue"
        relRawDataProcessorApiInterface = rawDataProcessor -> apiInterface "Requests data for processing"
        relRawDataProcessorLogDatabase = rawDataProcessor -> logDatabase "Logs and errors to"
        
        restAPI -> workloadQueue "Adds a workload message to"
        relRestAPILogDatabase = restAPI -> logDatabase "Logs and errors to"
        
        dataAnalysisVisualization -> logDatabase "Reads data to create visual dashboards"
    }

    views {
        systemContext system {
            include *
            autolayout lr
        }

        container system {
            include *
            
        }
        
        component frontendContainer {
            include *
            
        }
        
        component apiGateway {
            include *
            
        }
        
        component authContainer {
            include *
            autolayout lr
        }

        component dataCollector {
            include *
            exclude relApiInterfaceLogDatabase relConsumerFomattedDataLogDatabase
            
        }

        component dataStorage {
            include *
            exclude  apiGatewayRestAPI relRawDataProcessorLogDatabase relRestAPILogDatabase dataSourceConnectorsLogDatabase dataPublisherLogDatabase
            
        }
        
        component dataProcessing {
            include *
            exclude relConsumerFomattedDataLogDatabase apiGatewaydataStorage
            
        }
        
        component errorHandling {
            include *
            exclude relRawDataProcessorApiInterface consumerFomattedDataMessageBroker
            
        }

        theme default
        
        styles {
            element "frontendContainer" {
                background "#3C78D8"
                color "#ffffff"
            }
            element "apiGateway" {
                background "#FF8C00"
                color "#ffffff"
            }
			element "routingComponent" {
                background "#FF8C00"
                color "#ffffff"
            }
			element "securityComponent" {
                background "#FF8C00"
                color "#ffffff"
            }
			element "loggingComponent" {
                background "#FF8C00"
                color "#ffffff"
            }
            element "authContainer" {
                background "#6D9EEB"
                color "#ffffff"
            }
            element "dataCollector" {
                background "#6D9EEB"
                color "#ffffff"
            }
            element "messageBroker" {
                background "#B6D7A8"
                color "#000000"
            }
            element "dataStorage" {
                background "#6D9EEB"
                color "#ffffff"
            }
            element "dataProcessing" {
                background "#6D9EEB"
                color "#ffffff"
            }
            element "externalDataSource" {
                background "#84bb26"
                color "#ffffff"
            }
            element "dataSourceConnectors" {
                background "#F9CB9C"
                color "#000000"
            }
            element "dataIngestionEngine" {
                background "#B7E1CD"
                color "#000000"
            }
            element "dataTransformation" {
                background "#FFE599"
                color "#000000"
            }
            element "temporarilyRepository" {
                background "#00A8E8"
                color "#ffffff"
				shape Cylinder
            }
            element "errorHandling" {
                background "#6D9EEB"
                color "#ffffff"
            }
            element "dataPublisher" {
                background "#F9CB9C"
                color "#000000"
            }
            element "timeSeriesDB" {
                background "#00A8E8"
                color "#ffffff"
				shape Cylinder
            }
            element "apiInterface" {
                background "#FF8C00"
                color "#ffffff"
            }
            element "encryptionModule" {
                background "#2E8B57"
                color "#ffffff"
            }
			element "consumerFomattedData" {
                background "#F9CB9C"
                color "#000000"
            }
            element "rawDataProcessor" {
                background "#F9CB9C"
                color "#000000"
            }
            element "timeSeriesDB2" {
                background "#00A8E8"
                color "#ffffff"
				shape Cylinder
            }
            element "logDatabase" {
                background "#00A8E8"
                color "#ffffff"
				shape Cylinder
            }
            element "authServiceRepository"{
                background "#00A8E8"
                color "#ffffff"
				shape Cylinder
            }
			element "authServiceAPI"{
				background "#FF8C00"
                color "#ffffff"
			}
			element "restAPI"{
				background "#FF8C00"
                color "#ffffff"
			}
			element "workloadQueue"{
				background "#B6D7A8"
                color "#000000"
			}
			element "dataAnalysisVisualization"{
				background "#F9CB9C"
                color "#000000"
			}
			element "dashboard"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "authComponent"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "settingsComponent"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "dataVizComponent"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "notificationComponent"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "dataControlComponent"{
				background "#3C78D8"
                color "#ffffff"
			}
			element "profileComponent"{
				background "#3C78D8"
                color "#ffffff"
			}

        }
    }
}
