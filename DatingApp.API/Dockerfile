FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 8080

# COPY CSPROJ AND RESTORE AS DISTINCT LAYERS
COPY *.csproj ./
RUN dotnet restore

# COPY EVERYTHING ELSE AND BUILD
COPY . ./
RUN dotnet publish -c Release -o out

# BUILD RUNTIME IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "DatingApp.API.dll" ]






## ADD TO PRODUCTION APPSETTINGS ##
###################################
##  "ConnectionStrings": {
##    "DefaultConnection": "Server=localhost,1433;Database=DatingApp;User ID=sa;Password=1q2w3e4r@#$;Trusted_Connection=False; ##TrustServerCertificate=True;"
##  },
##  "TokenKey": "ABCD!12345-ABCD!12345-ABCD!12345-ABCD!12345-ABCD!12345-ABCD!12345"


## YET TO DO (CREATE IMAGE) ##
##############################
## docker build -t igorsantiago/datingapp .

## RUN IMAGE CREATED ##
#######################
## docker run --rm -it -p 8080:80 igorsantiago/datingapp:latest



## ADD TO DOCKER HUB ##
#######################
## docker login
## docker push igorsantiago/datingapp:latest