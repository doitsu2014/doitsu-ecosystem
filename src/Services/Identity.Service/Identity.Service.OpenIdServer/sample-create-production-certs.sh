mkdir -p certs
mkdir -p certs/root

# Parameter
# CN=$CN
# COUNTRY=$COUNTRY
# STATE=$STATE
# LOCATION=$LOCATION
# OU=$OU
# PW=$PASSWORD

CN="identity.doitsu.tech"
COUNTRY="VN"
STATE="HCM"
LOCATION="HCM"
OU="DTech"
PW=$PW

# Create root
openssl req -new -x509 \
-outform PEM \
-out certs/root/ca.pem \
-keyout ./certs/root/ca.key \
-days 3650 -nodes \
-passin pass:$PW \
-subj "/C=$COUNTRY/ST=$STATE/L=$OU/O=$OU/CN=$CN"

# export pfx
openssl pkcs12 -export \
-out certs/$CN.pfx \
-inkey certs/root/ca.key \
-passin pass:$PW \
-passout pass:$PW \
-in certs/root/ca.pem
