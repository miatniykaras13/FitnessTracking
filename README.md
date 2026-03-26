Чтобы запустить: 

1. Добавить в User Secrets:
   
    "ConnectionStrings": {
      "FitnessTrackingDbContext": "Server=localhost;Database=FitnessTrackingDb;User Id=postgres;Password=1111"
    },
    "Kestrel:Certificates:Default:Password": "1111",
    "Auth": {
      "Secret": "ThisIsASecretKeyForJwtTokenGeneration",
      "Issuer": "FitnessTracking.Api",
      "Audience": "FitnessTracking.Client",
      "ExpiresInMinutes": 60
    }

3. Создать .env файл в FitnessTracking со следующим содержимым:
   
  FITNESSTRACKING_POSTGRES_USER=postgres
  
  FITNESSTRACKING_POSTGRES_PASSWORD=1111
  
  FITNESSTRACKING_POSTGRES_DB=FitnessTrackingDb
   
