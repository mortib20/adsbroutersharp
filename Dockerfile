from alpine:latest as build
WORKDIR /build
COPY . .
RUN apk add --no-cache dotnet7-sdk
RUN dotnet publish -c Release -r linux-musl-x64 --self-contained false -o ./bin .

from alpine:latest as main
WORKDIR /router
COPY --from=build /build/bin/Release/net7.0/linux-musl-x64 /router
RUN apk add --no-cache aspnetcore7-runtime

CMD ["./adsbrouter"]