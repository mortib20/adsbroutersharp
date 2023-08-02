from alpine:latest as build
WORKDIR / build
COPY . .
RUN apk add --no-cache dotnet7-sdk
RUN dotnet publish -c Release -o ./bin .

from alpine:latest as main
WORKDIR / router
COPY --from=build / build / bin / Release / net7.0 / / router
RUN apk add --no-cache aspnetcore7-runtime

CMD ["./ADSBRouterSharpv2"]