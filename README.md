# Herramientas de trabajo comunes
Con el fin de evitar problemas de compatibilidad y posibles errores vamos a intentar descargarnos las mismas herramientas, que son las siguientes:
- .NET SDK 7.0: https://dotnet.microsoft.com/es-es/download

![image](https://user-images.githubusercontent.com/121343480/217650586-360a360d-6bdf-4813-8444-93a5ede8ce80.png)

- Visual Studio Code: https://code.visualstudio.com/Download
	- [C# Sharp Plugin](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp "C# Sharp Plugin")
	- [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions "C# Extensions")

# ☢️😮 [Importante] Forma de trabajo
La forma de trabajar será en base a una rama main en la que admitiremos unicamente el código bueno y una rama por cada programador en base a la main. Con esto conseguiremos copias de seguridad en caso de haber perdidas, no nos pisamos el código y únicamente se admitira el código bueno cuando se acepte un Pull Request.

![git](https://user-images.githubusercontent.com/121343480/217650709-992c26c4-49d6-4130-8e93-56b2c0b86456.png)

## Como crear una rama en VSCode
Después de haber hecho fork, y tener "cloneado" el proyecto de la rama main. Vamos a crearnos una rama llamada: `rama_tunombre` de esta manera:

![image](https://user-images.githubusercontent.com/121343480/217652348-dd386584-ad61-47be-9a46-ccf819114513.png)

**CREAR RAMA DESDE ORIGIN/MAIN** 🚩🚩🚩🚩🚩🚩🚩

![image](https://user-images.githubusercontent.com/121343480/217652670-650eb87e-e1a3-4008-a0fd-7fd842eb6644.png)

![image](https://user-images.githubusercontent.com/121343480/217652714-6cc93969-c3da-4fcc-bf6e-e294a8cbfa2c.png)

![image](https://user-images.githubusercontent.com/121343480/217652830-45aa00ec-40a9-40fb-bb62-d60da2e4b1e4.png)

![image](https://user-images.githubusercontent.com/121343480/217653441-fd7eec24-193b-4c6c-a318-10526be1004a.png)

A partir de aquí podemos empezar a programar sin problema.

## Mi código funciona, ¿cómo lo paso a la rama main?

![image](https://user-images.githubusercontent.com/121343480/217656110-e523ec4c-56ac-4edd-8070-2e9b922fed71.png)

![image](https://user-images.githubusercontent.com/121343480/217656364-d8183211-5ef4-4d6d-bc23-66c13f069fc3.png)

Vamos a nuestra repo en GitHub

![image](https://user-images.githubusercontent.com/121343480/217656581-cba34dd9-043a-4986-949e-df98a6b87c2d.png)

Nos fijamos que nuestra rama se pasa a la main
![image](https://user-images.githubusercontent.com/121343480/217656679-08d62526-66b9-43ef-bd66-64f0c5aff289.png)

Y luego create pull request y para finalizar:

![image](https://user-images.githubusercontent.com/121343480/217656804-904def3b-75e2-4ca5-a601-2a9e3f7b6336.png)

## Han fusionado código y ahora el mio esta desactualizado en local. Como sincronizar.

![image](https://user-images.githubusercontent.com/121343480/217659415-008a7cd9-6a22-42a4-89cd-a45d3b29ea5a.png)

Podemos extraer desde la rama origin/main

![image](https://user-images.githubusercontent.com/121343480/217659612-cfd06abb-2104-43cf-bd91-33a3b3b18eb1.png)
