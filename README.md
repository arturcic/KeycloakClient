.NET KeycloakClient Admin client

| Build server    | Platform | Build status                                                     |
|-----------------|----------|------------------------------------------------------------------|
| Azure Pipelines | Windows  | [![Build Status][azure-pipeline-badge-windows] ][azure-pipeline] |
| Azure Pipelines | Linux    | [![Build Status][azure-pipeline-badge-linux] ][azure-pipeline]   |
| Azure Pipelines | macOS    | [![Build Status][azure-pipeline-badge-macos] ][azure-pipeline]   |

[![codecov](https://codecov.io/gh/arturcic/KeycloakClient/branch/master/graph/badge.svg)](https://codecov.io/gh/arturcic/KeycloakClient)

[azure-pipeline]:                https://dev.azure.com/arturcic/OSS/_build/latest?definitionId=7&branchName=master
[azure-pipeline-badge-windows]:  https://dev.azure.com/arturcic/OSS/_apis/build/status/KeycloakClient?branchName=master&jobName=Build&configuration=Build%20Windows
[azure-pipeline-badge-linux]:    https://dev.azure.com/arturcic/OSS/_apis/build/status/KeycloakClient?branchName=master&jobName=Build&configuration=Build%20Linux
[azure-pipeline-badge-macos]:    https://dev.azure.com/arturcic/OSS/_apis/build/status/KeycloakClient?branchName=master&jobName=Build&configuration=Build%20macOS

Inspired by [Java admin client](https://github.com/keycloak/keycloak/tree/master/integration/admin-client/src/main/java/org/keycloak/admin/client)

## Features

* Keycloak v4.3 supported

## Install

```sh
dotnet add package keycloak.admin.client --version 1.0.0
```

## Usage

```c#
// add in ConfigureService for Asp.Net Core apps
services.AddKeycloakAdminClient(options =>
{
    options.Address  = new Uri("http://localhost:8080/auth/");
    options.Realm    = "master";
    options.Username = "username";
    options.Password = "pasword";
    options.ClientId = "admin-cli";
});

// inject the client 
public SomeController(IKeycloakClient keycloakClient)
{
    _keycloakClient = keycloakClient
}

// or manually create
_keycloakClient = new KeycloakClient(options);

// create client
var clientsResource = _keycloakClient
    .Realms().Id(realmName)
    .Clients();

var client = new Client
{
    ClientId = "app-client",
    Name = "App Client",
    Enabled = true,
    StandardFlowEnabled = true,
    DirectAccessGrantsEnabled = true,
    RedirectUris = new[] {"*"},
    WebOrigins = new[] {"http://localhost:5000"} // this is the app that integrates with the keycloak
};

var clientCreated = await clientsResource.CreateAsync(client);

```

## Supported APIs

### [Realm admin](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_realms_admin_resource)

|   Method 	| Url pattern 	| Description                                             	|
|---------:	|-------------	|---------------------------------------------------------	|
|   `POST` 	| `/`         	| Import a realm from a full representation of that realm 	|
|    `GET` 	| `/`         	| Returns a list of accessible realms                     	|
|    `GET` 	| `/{realm}`  	| Get the top-level representation of the realm           	|
|    `PUT` 	| `/{realm}`  	| Update the top-level information of the realm           	|
| `DELETE` 	| `/{realm}`  	| Delete the realm                                        	|


### [User](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_users_resource)

|   Method 	| Url pattern                                	| Description                                                                                                              	|
|---------:	|-------------------------------------------- 	|--------------------------------------------------------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/users`                           	| Create a new user                                                                                                        	|
|    `GET` 	| `/{realm}/users`                           	| Get a list of users, filtered according to query parameters                                                              	|
|    `GET` 	| `/{realm}/users/{id}`                      	| Get representation of the user                                                                                           	|
|    `PUT` 	| `/{realm}/users/{id}`                      	| Update the user                                                                                                          	|
| `DELETE` 	| `/{realm}/users/{id}`                      	| Delete the user                                                                                                          	|
|    `GET` 	| `/{realm}/users/{id}/groups`               	| Get user groups                                                                                                          	|
|    `PUT` 	| `/{realm}/users/{id}/groups/{groupId}`     	| Add user to group                                                                                                        	|
| `DELETE` 	| `/{realm}/users/{id}/groups/{groupId}`     	| Delete user from group                                                                                                   	|
|    `PUT` 	| `/{realm}/users/{id}/reset-password`       	| Set up a temporary password for the user User will have to reset the temporary password next time they log in            	|
|    `PUT` 	| `/{realm}/users/{id}/send-verify-email`    	| Send an email-verification email to the user An email contains a link the user can click to verify their email address   	|
|    `PUT` 	| `/{realm}/users/{id}/execute-actions-email`	| Send a update account email to the user An email contains a link the user can click to perform a set of required actions 	|


### [Group](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_groups_resource)

|   Method 	| Url pattern                    	| Description                                                                	|
|---------:	|--------------------------------	|----------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/groups`              	| Create a new group                                                         	|
|    `GET` 	| `/{realm}/groups`              	| Get groups, filtered according to query parameters                         	|
|    `GET` 	| `/{realm}/groups/{id}`         	| Get representation of the group                                            	|
|    `PUT` 	| `/{realm}/groups/{id}`         	| Update the group                                                           	|
| `DELETE` 	| `/{realm}/groups/{id}`         	| Delete the group                                                           	|
|    `GET` 	| `/{realm}/groups/{id}/members` 	| Get group users                                                            	|


### [Client](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_clients_resource)

|   Method 	| Url pattern             	| Description                        	|
|---------:	|-------------------------	|------------------------------------	|
|   `POST` 	| `/{realm}/clients`      	| Create a new client                	|
|    `GET` 	| `/{realm}/clients`      	| Get clients belonging to the realm 	|
|    `GET` 	| `/{realm}/clients/{id}` 	| Get representation of the client   	|
|    `PUT` 	| `/{realm}/clients/{id}` 	| Update the client                  	|
| `DELETE` 	| `/{realm}/clients/{id}` 	| Delete the client                  	|


### [Realm Roles](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_roles_resource)

|   Method 	| Url pattern                                               	| Description                                                                           	|
|---------:	|-----------------------------------------------------------	|---------------------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/roles`                                          	| Create a new role for the realm                                                       	|
|    `GET` 	| `/{realm}/roles`                                          	| Get all roles for the realm                                                           	|
|    `GET` 	| `/{realm}/roles/{role-name}`                              	| Get a role by name                                                                    	|
|    `PUT` 	| `/{realm}/roles/{role-name}`                              	| Update a role by name                                                                 	|
| `DELETE` 	| `/{realm}/roles/{role-name}`                              	| Delete a role by name                                                                 	|
|    `GET` 	| `/{realm}/roles/{role-name}/composite`                    	| Get role's children Returns a set of role's children provided the role is a composite.	|
|   `POST` 	| `/{realm}/roles/{role-name}/composite`                    	| Make the role a composite role by associating some child roles                        	|
| `DELETE` 	| `/{realm}/roles/{role-name}/composite`                    	| Remove a set of roles from the role's composite                                       	|
|    `GET` 	| `/{realm}/roles/{role-name}/composites/realm`             	| Get realm-level roles that are in the role's composite                                	|
|    `GET` 	| `/{realm}/roles/{role-name}/composites/clients/{clientId}`	| Get client-level roles for the client that are in the role's composite               		|
|    `GET` 	| `/{realm}/roles/{role-name}/users`                        	| Return List of Users that have the specified role name                                	|


### [Roles (by ID)](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_roles_by_id_resource)

|   Method 	| Url pattern                                                   	| Description                                                                           	|
|---------:	|---------------------------------------------------------------	|---------------------------------------------------------------------------------------	|
|    `GET` 	| `/{realm}/roles-by-id/{role-id}`                              	| Get a specific role by Id                                                             	|
|    `PUT` 	| `/{realm}/roles-by-id/{role-id}`                              	| Update a specific role by Id                                                          	|
| `DELETE` 	| `/{realm}/roles-by-id/{role-id}`                              	| Delete a specific role by Id                                                          	|
|    `GET` 	| `/{realm}/roles-by-id/{role-id}/composite`                    	| Get role's children Returns a set of role's children provided the role is a composite.	|
|   `POST` 	| `/{realm}/roles-by-id/{role-id}/composite`                    	| Make the role a composite role by associating some child roles                        	|
| `DELETE` 	| `/{realm}/roles-by-id/{role-id}/composite`                    	| Remove a set of roles from the role's composite                                       	|
|    `GET` 	| `/{realm}/roles-by-id/{role-id}/composites/realm`             	| Get realm-level roles that are in the role's composite                                	|
|    `GET` 	| `/{realm}/roles-by-id/{role-id}/composites/clients/{clientId}`	| Get client-level roles for the client that are in the role's composite                	|


### [Client roles](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_roles_resource)

|   Method 	| Url pattern                                                            	| Description                                                                           	|
|---------:	|------------------------------------------------------------------------	|---------------------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/clients/{id}/roles`                                          	| Create a new role for the client                                                      	|
|    `GET` 	| `/{realm}/clients/{id}/roles`                                          	| Get roles belonging to the client                                                     	|
|    `GET` 	| `/{realm}/clients/{id}/roles/{role-name}`                              	| Get representation of the role for the client                                         	|
|    `PUT` 	| `/{realm}/clients/{id}/roles/{role-name}`                              	| Update the role for the client                                                        	|
| `DELETE` 	| `/{realm}/clients/{id}/roles/{role-name}`                              	| Delete the role for the client                                                        	|
|    `GET` 	| `/{realm}/clients/{id}/roles/{role-name}/composite`                    	| Get role's children Returns a set of role's children provided the role is a composite.	|
|   `POST` 	| `/{realm}/clients/{id}/roles/{role-name}/composite`                    	| Make the role a composite role by associating some child roles                        	|
| `DELETE` 	| `/{realm}/clients/{id}/roles/{role-name}/composite`                    	| Remove a set of roles from the role's composite                                       	|
|    `GET` 	| `/{realm}/clients/{id}/roles/{role-name}/composites/realm`             	| Get realm-level roles that are in the role's composite                                	|
|    `GET` 	| `/{realm}/clients/{id}/roles/{role-name}/composites/clients/{clientId}`	| Get client-level roles for the client that are in the role's composite                	|
|    `GET` 	| `/{realm}/clients/{id}/roles/{role-name}/users`                        	| Return List of Users that have the specified role name                                	|


### [User role-mapping](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_role_mapper_resource)

|   Method 	| Url pattern                                         	| Description                                                               	|
|---------:	|-----------------------------------------------------	|---------------------------------------------------------------------------	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings`                 	| Get user role-mappings                                                    	|
|   `POST` 	| `/{realm}/users/{id}/role-mappings/realm`           	| Add realm-level role mappings to the user                                 	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/realm`           	| Get realm-level role mappings for a user                                  	|
| `DELETE` 	| `/{realm}/users/{id}/role-mappings/realm`           	| Delete realm-level role mappings for a user                               	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/realm/available` 	| Get realm-level roles that can be mapped for a user                       	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/realm/composite` 	| Get effective realm-level role mappings This recurses any composite roles 	|


### [Client role-mapping for user](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_role_mappings_resource)

|   Method 	| Url pattern                                                    	| Description                                                                	|
|---------:	|----------------------------------------------------------------	|----------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/users/{id}/role-mappings/clients/{client}`           	| Add client-level roles to the user role mapping                            	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/clients/{client}`           	| Get client-level role mappings for the user                                	|
| `DELETE` 	| `/{realm}/users/{id}/role-mappings/clients/{client}`           	| Delete client-level roles from group user mapping                          	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/clients/{client}/available` 	| Get available client-level roles that can be mapped to the user            	|
|    `GET` 	| `/{realm}/users/{id}/role-mappings/clients/{client}/composite` 	| Get effective client-level role mappings This recurses any composite roles 	|


### [Group role-mapping](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_role_mapper_resource)

|   Method 	| Url pattern                                          	| Description                                                               	|
|---------:	|------------------------------------------------------	|---------------------------------------------------------------------------	|
|    `Get` 	| `/{realm}/groups/{id}/role-mappings`                 	| Get group role-mappings                                                   	|
|   `POST` 	| `/{realm}/groups/{id}/role-mappings/realm`           	| Add realm-level role mappings to the group                                	|
|    `Get` 	| `/{realm}/groups/{id}/role-mappings/realm`           	| Get realm-level role mappings for the group                               	|
| `DELETE` 	| `/{realm}/groups/{id}/role-mappings/realm`           	| Delete realm-level role mappings for the group                            	|
|    `Get` 	| `/{realm}/groups/{id}/role-mappings/realm/available` 	| Get realm-level roles that can be mapped for the group                    	|
|    `Get` 	| `/{realm}/groups/{id}/role-mappings/realm/composite` 	| Get effective realm-level role mappings This recurses any composite roles 	|

### [Client role-mapping for group](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_role_mappings_resource)

|   Method 	| Url pattern                                                     	| Description                                                               	|
|---------:	|-----------------------------------------------------------------	|---------------------------------------------------------------------------	|
|   `POST` 	| `/{realm}/groups/{id}/role-mappings/clients/{client}`           	| Add client-level roles to the group role mapping                          	|
|    `GET` 	| `/{realm}/groups/{id}/role-mappings/clients/{client}`           	| Get client-level role mappings for the group                              	|
| `DELETE` 	| `/{realm}/groups/{id}/role-mappings/clients/{client}`           	| Delete client-level roles from group role mapping                         	|
|    `GET` 	| `/{realm}/groups/{id}/role-mappings/clients/{client}/available` 	| Get available client-level roles that can be mapped to the group          	|
|    `GET` 	| `/{realm}/groups/{id}/role-mappings/clients/{client}/composite` 	| Get effective client-level role mappings This recurses any composite roles	|


## Not yet supported
* [Attack Detection](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_attack_detection_resource)
* [Authentication Management](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_authentication_management_resource)
* [Client Attribute Certificate](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_attribute_certificate_resource)
* [Client Initial Access](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_initial_access_resource)
* [Client Registration Policy](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_registration_policy_resource)
* [Client Scopes](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_client_scopes_resource)
* [Key](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_key_resource)
* [Protocol Mappers](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_protocol_mappers_resource)
* [Scope Mappings](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_scope_mappings_resource)
* [User Storage Provider](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_user_storage_provider_resource)
* [Identity Providers](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_identity_providers_resource)
* [Component](https://www.keycloak.org/docs-api/4.3/rest-api/index.html#_component_resource)
