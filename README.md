# authorization-api
Authorization API with sample Client API using IdentityServer4 JWT authorization. Authorization request using json body. 

Request to auth.api example : 

path: https://localhost:5456/token/authorize
body: {
    "username" : "username",
    "password" : "password",
    "clientId" : "client_id",
    "clientSecret" : "client_secret"
}
