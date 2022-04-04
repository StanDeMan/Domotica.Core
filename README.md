# Domotica.Core
SignalR Remoting (pigpiod installation needed on Raspberry Pi) LED Stripe.
Install pigpiod. It's an amazing piece of software: https://abyz.me.uk/rpi/pigpio/pigpiod.html
Even PWM is possible with low CPU power consumtion - doing it via DMA.

Used self signed certificate

```
# Default NGiNX server configuration
# 
server {
   listen 80;
   server_name domotica.net.local *.domotica.net.local;
   return 301 https://$host$request_uri;
}

server {
   listen               443;
   server_name          domotica.net.local *.domotica.net.local;
   ssl_certificate      /home/pi/mkcert/domotica.net.local.pem;
   ssl_certificate_key  /home/pi/mkcert/domotica.net.local-key.pem;

   ssl on;
   ssl_session_cache  builtin:1000  shared:SSL:10m;
   ssl_protocols  TLSv1 TLSv1.1 TLSv1.2 TLSv1.3;
   ssl_ciphers HIGH:!aNULL:!eNULL:!EXPORT:!CAMELLIA:!DES:!MD5:!PSK:!RC4;
   ssl_prefer_server_ciphers on;

   location / {
      proxy_pass         https://localhost:5001;
      proxy_http_version 1.1;
      proxy_set_header   Upgrade $http_upgrade;
      proxy_set_header   Connection keep-alive;
      proxy_set_header   Host $host;
      proxy_cache_bypass $http_upgrade;
      proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
      proxy_set_header   X-Forwarded-Proto $scheme;
   }

   location /hubs/device {
      proxy_pass https://localhost:5001;
      proxy_http_version 1.1;
      proxy_set_header   Upgrade $http_upgrade;
      proxy_set_header   Connection keep-alive;
      proxy_set_header   Connection "upgrade";
      proxy_set_header   Host $host;
      proxy_cache_bypass $http_upgrade;
      proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
      proxy_set_header   X-Forwarded-Proto $scheme;
   }
}
```
