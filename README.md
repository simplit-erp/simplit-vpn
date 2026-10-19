# SimplitVPN para Windows

Aplicación gráfica nativa para configurar y conectar la VPN IKEv2 de Oilmetal usando el cliente integrado de Windows. El usuario ejecuta un `.exe` con una ventana normal de Windows; no tiene que abrir PowerShell ni editar un perfil manualmente.

El asistente crea o actualiza el perfil VPN del usuario actual, habilita túnel dividido, agrega la ruta `172.16.0.0/24`, configura el sufijo DNS `oilmetal.com.ar` y no almacena credenciales. Windows solicita las credenciales al conectar.

## Obtener el ejecutable

Descarga `SimplitVPN.exe` desde **Releases**. Al ser una primera versión sin firma digital, Windows puede mostrar una advertencia de editor desconocido.

## Uso

1. Ejecuta `SimplitVPN.exe`.
2. Pulsa **Configurar VPN** una vez.
3. Pulsa **Conectar** e ingresa tus credenciales cuando Windows las solicite.

La aplicación requiere Windows 10 u 11. No instala certificados, no cambia el servidor y no requiere permisos de administrador para un perfil del usuario actual. Si el equipo ya tiene un perfil VPN para todos los usuarios, puede requerir intervención de un administrador.

## Publicar una versión

El workflow de GitHub Actions compila el ejecutable en Windows y lo adjunta a un Release al publicar un tag con formato `v*` (por ejemplo `v1.0.4`). El proyecto debe estar en un repositorio GitHub con Actions habilitado. No subir contraseñas, claves privadas ni configuraciones secretas.
