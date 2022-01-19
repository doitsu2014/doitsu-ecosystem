Version=$(git log --format="%h" -n 1)
Repository="services.identity"
Registry="docker-hub.doitsu.tech"

docker build -f Services/Identity.Service/Identity.Service.OpenIdServer/Dockerfile -t $Repository . 

docker build . -t $Repository
docker image tag $Repository "$Registry/$Repository:latest"
docker image tag $Repository "$Registry/$Repository:$Version"
docker image push --all-tags "$Registry/$Repository"