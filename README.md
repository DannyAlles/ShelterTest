Подняла БД в докере:
Хост: localhost
Порт: 1433
Пользователь: sa
Пароль: P@ssword
В конфиге сервера:
_"DefaultConnection": "Server=192.168.179.183,1433;Database=ShelterTest;User ID=sa;Password=P@ssword;TrustServerCertificate=true;"_

Схема в БД для скриптов тоже стандартная - dbo

В качестве тела к запросам использую следующий json формат:

_{
  "operation": string,
  "id": string,
  "body": 
  {
    "name": string,
    "inn": string,
    "phone": stirng,
    "parentCompanyId": string
  }
}_

Для CREATE можно не указывать "_parentCompanyId_" и "_id_"
Для UPDATE можно не указывать "_parentCompanyId_", тогда он будет принят как _null_
Для получения списка хватает только "_operation_": "_READ_LIST_"
Для получения и удаления конкретной компании необходимо указать _id_

Файл коллекции постман находится в корневой папке решения, скрипты находятся в директории /_DB_/_Scriptss_

Для авторизации в заголовок нужно добавить Authorization со значением i+sUMoktgV8mR!pUJwk_iV_WijO$v&
