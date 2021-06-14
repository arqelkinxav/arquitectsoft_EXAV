-- MySQL dump 10.13  Distrib 8.0.25, for Win64 (x86_64)
--
-- Host: localhost    Database: arquitectdb
-- ------------------------------------------------------
-- Server version	8.0.25

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `acabados`
--

DROP TABLE IF EXISTS `acabados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `acabados` (
  `Id_Acabado` int NOT NULL AUTO_INCREMENT,
  `Codigo_Homologacion` varchar(2) NOT NULL,
  `Descripcion` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_Acabado`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `acabados`
--

LOCK TABLES `acabados` WRITE;
/*!40000 ALTER TABLE `acabados` DISABLE KEYS */;
INSERT INTO `acabados` VALUES (1,'11','LACADO NEGRO 9005 MATE LISO'),(2,'1','ANODIZADO PLATA MATE'),(3,'12','LACADO ANTRACITA 7016 MATE LISO'),(4,'10','LACADO BLANCO 9003 MATE LISO'),(5,'50','GALVANIZADO'),(6,'51','CINCADO NATURAL'),(7,'61','PLASTICO TRANSPARENTE'),(8,'62','PLASTICO BLANCO'),(9,'63','PLASTICO GRIS PLATA'),(10,'64','PLASTICO NEGRO'),(11,'65','PLASTICO VERDE'),(12,'66','PLASTICO AZUL'),(13,'70','INOX'),(14,'71','ACERO'),(15,'72','BRUTO'),(16,'75','LANA'),(17,'80','CINTA ADHESIVA'),(18,'81','ACRILICA GRIS'),(19,'82','ACRILICA ANTRACITA'),(20,'83','TELA GRIS'),(21,'84','TELA NEGRO'),(22,'85','LIQUIDO'),(23,'86','SILICONA'),(24,'87','ADHESIVO'),(25,'90','VIDRIO TRANSPARENTE'),(26,'91','VIDRIO TRANSLÚCIDO'),(27,'95','DM'),(28,'96','AGLOMERADO'),(29,'97','CONTRACHAPADO'),(30,'98','MADERA MACIZA');
/*!40000 ALTER TABLE `acabados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categorias`
--

DROP TABLE IF EXISTS `categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categorias` (
  `Id_Categoria` int NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(100) NOT NULL,
  `Tipo_Formula` varchar(2) NOT NULL,
  PRIMARY KEY (`Id_Categoria`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categorias`
--

LOCK TABLES `categorias` WRITE;
/*!40000 ALTER TABLE `categorias` DISABLE KEYS */;
INSERT INTO `categorias` VALUES (1,'Perfiles','A'),(2,'Puertas','B'),(3,'Paneles y Vidrios','C');
/*!40000 ALTER TABLE `categorias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `componentes`
--

DROP TABLE IF EXISTS `componentes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `componentes` (
  `Id_Componente` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(20) NOT NULL,
  `Descripcion` varchar(200) NOT NULL,
  `NoSubcomponente` tinyint NOT NULL DEFAULT '0',
  `Fecha_Creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Especial` int NOT NULL,
  PRIMARY KEY (`Id_Componente`),
  UNIQUE KEY `Codigo_UNIQUE` (`Codigo`),
  UNIQUE KEY `Descripcion_UNIQUE` (`Descripcion`)
) ENGINE=InnoDB AUTO_INCREMENT=90 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `componentes`
--

LOCK TABLES `componentes` WRITE;
/*!40000 ALTER TABLE `componentes` DISABLE KEYS */;
INSERT INTO `componentes` VALUES (58,'TUB100100','TUBO METALICO',0,'2021-03-20 19:09:33',0),(76,'DVS0004-61','PERFIL DE DV ARRANQUE DE PARED',0,'2021-05-27 19:10:02',0),(77,'DVS0007-61','PERFIL DE DV PARA SUELO',0,'2021-05-27 19:20:44',0),(80,'DVS0009-61','PERFIL DE DV PARA TECHO',0,'2021-05-27 19:53:34',0),(81,'AVS0004-61','PERFIL DE AV ARRANQUE DE PARED',0,'2021-05-27 20:37:27',0),(82,'AVS0007-61','PERFIL DE AV PARA SUELO',0,'2021-05-27 20:38:26',0),(83,'AVS0008-61','PERFIL DE U PARA SUELO',0,'2021-05-27 20:39:03',0),(84,'AVS0009-61','PERFIL DE AV PARA TECHO',0,'2021-05-27 20:39:39',0),(86,'AVS0010-61','PERFIL DE AV A REMATE',0,'2021-05-27 20:42:59',0),(87,'PREM-61','PERFIL DE REMATE 100 X 20',0,'2021-05-27 20:46:55',0),(88,'LCT001','Prueba',0,'2021-06-13 11:36:03',0),(89,'ESP001','PRUEBA DOS',0,'2021-06-14 12:11:53',1);
/*!40000 ALTER TABLE `componentes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `componentes_detalle`
--

DROP TABLE IF EXISTS `componentes_detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `componentes_detalle` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Id_Componente` int NOT NULL,
  `Id_Subcomponente` int NOT NULL,
  `Id_Unidad_Calculada` int NOT NULL DEFAULT '1',
  `Cantidad_Default` int NOT NULL DEFAULT '1',
  `Cantidad_Adicional` int NOT NULL DEFAULT '30',
  `Aplica_Decremento` tinyint NOT NULL DEFAULT '0',
  `elevado` int NOT NULL DEFAULT '0',
  `idCorte` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=304 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `componentes_detalle`
--

LOCK TABLES `componentes_detalle` WRITE;
/*!40000 ALTER TABLE `componentes_detalle` DISABLE KEYS */;
INSERT INTO `componentes_detalle` VALUES (197,58,371,5,1,30,0,0,0),(259,77,151,1,1,30,0,0,0),(260,77,153,1,1,30,0,0,0),(261,80,78,1,1,30,0,0,0),(262,80,151,1,1,30,0,0,0),(263,80,153,1,1,30,0,0,0),(268,82,155,1,1,30,0,0,0),(269,83,156,1,1,30,0,0,0),(272,76,151,5,1,30,0,0,0),(273,76,78,5,1,30,0,0,0),(274,76,153,5,1,30,0,0,0),(275,81,155,5,1,30,0,0,0),(276,81,78,5,1,30,0,0,0),(278,86,155,5,1,30,0,0,0),(286,87,374,5,1,30,0,0,0),(287,87,77,5,1,30,0,0,0),(288,87,118,3,2,30,0,0,0),(289,84,78,1,1,30,0,0,0),(290,84,155,1,1,30,0,0,0),(295,88,9,4,2,45,1,2,2),(296,88,12,3,1,30,0,0,1),(297,-1,11,3,1,30,1,0,2),(298,-1,13,2,1,30,1,0,2),(303,89,10,2,2,35,1,1,2);
/*!40000 ALTER TABLE `componentes_detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `componentes_especial`
--

DROP TABLE IF EXISTS `componentes_especial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `componentes_especial` (
  `Id_Componente_especial` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(20) NOT NULL,
  `Descripcion` varchar(200) NOT NULL,
  `Fecha_Creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id_Componente_especial`),
  UNIQUE KEY `Codigo_UNIQUE` (`Codigo`),
  UNIQUE KEY `Descripcion_UNIQUE` (`Descripcion`)
) ENGINE=InnoDB AUTO_INCREMENT=88 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `componentes_especial`
--

LOCK TABLES `componentes_especial` WRITE;
/*!40000 ALTER TABLE `componentes_especial` DISABLE KEYS */;
INSERT INTO `componentes_especial` VALUES (72,'AVA0801','IMPULS 100 AV MONOVIDRIO','2021-03-15 16:46:04'),(73,'DVA0001','PRUEBA DE VIDRIOS','2021-03-15 16:53:53'),(85,'AVS1201-61','DOOR_IMPULS 100 AV PUERTA CIEGA CAPIALZADA','2021-05-05 14:55:23'),(86,'AVA0802','VIDRIO 5+5MM ACABADO NATURAL','2021-05-12 14:53:59'),(87,'ITA0001','VIDRIO 6+6','2021-05-12 22:30:42');
/*!40000 ALTER TABLE `componentes_especial` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `componentes_especial_detalle`
--

DROP TABLE IF EXISTS `componentes_especial_detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `componentes_especial_detalle` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Id_Componente_especial` int NOT NULL,
  `Id_Subcomponente` int NOT NULL,
  `select_Columna` int NOT NULL DEFAULT '1',
  `Cantidad_Default` int DEFAULT '1',
  `Cantidad_Adicional` int DEFAULT '30',
  `Aplica_Decremento` tinyint DEFAULT '0',
  `elevado` int DEFAULT '0',
  `idCorte` int DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=202 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `componentes_especial_detalle`
--

LOCK TABLES `componentes_especial_detalle` WRITE;
/*!40000 ALTER TABLE `componentes_especial_detalle` DISABLE KEYS */;
INSERT INTO `componentes_especial_detalle` VALUES (170,73,368,1,1,30,0,0,0),(171,73,368,2,1,30,0,0,0),(172,72,370,1,1,30,0,0,0),(185,85,368,1,1,30,0,0,0),(186,85,369,1,1,30,0,0,0),(187,85,370,1,1,30,0,0,0),(188,86,370,2,1,30,0,0,0),(189,86,370,3,1,30,0,0,0),(190,86,370,4,1,30,0,0,0),(191,86,370,5,1,30,0,0,0),(193,87,373,1,1,30,0,0,0),(194,-1,9,1,4,3,0,0,0),(195,-1,9,2,3,2,0,0,0),(201,89,9,1,2,3,0,0,0);
/*!40000 ALTER TABLE `componentes_especial_detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cortes`
--

DROP TABLE IF EXISTS `cortes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cortes` (
  `Id_Corte` int NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(2) NOT NULL,
  `Corte_Derecho` int NOT NULL,
  `Corte_Izquierdo` int NOT NULL,
  PRIMARY KEY (`Id_Corte`),
  UNIQUE KEY `Descripcion_UNIQUE` (`Descripcion`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cortes`
--

LOCK TABLES `cortes` WRITE;
/*!40000 ALTER TABLE `cortes` DISABLE KEYS */;
INSERT INTO `cortes` VALUES (1,'Co',10,20),(2,'PR',2,23);
/*!40000 ALTER TABLE `cortes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proyecto`
--

DROP TABLE IF EXISTS `proyecto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proyecto` (
  `Id_Subcomponente` int NOT NULL,
  `Id_Unidad_Medida` int NOT NULL,
  `cantidad` int NOT NULL,
  `medida` decimal(18,2) NOT NULL,
  `medidaAdicional` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proyecto`
--

LOCK TABLES `proyecto` WRITE;
/*!40000 ALTER TABLE `proyecto` DISABLE KEYS */;
/*!40000 ALTER TABLE `proyecto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proyecto_mp`
--

DROP TABLE IF EXISTS `proyecto_mp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proyecto_mp` (
  `codigo` varchar(50) NOT NULL,
  `descripcion` varchar(400) NOT NULL,
  `medida` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proyecto_mp`
--

LOCK TABLES `proyecto_mp` WRITE;
/*!40000 ALTER TABLE `proyecto_mp` DISABLE KEYS */;
/*!40000 ALTER TABLE `proyecto_mp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proyecto_vidriopanel`
--

DROP TABLE IF EXISTS `proyecto_vidriopanel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proyecto_vidriopanel` (
  `Codigo` varchar(50) DEFAULT NULL,
  `Altura` int DEFAULT NULL,
  `Anchura` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proyecto_vidriopanel`
--

LOCK TABLES `proyecto_vidriopanel` WRITE;
/*!40000 ALTER TABLE `proyecto_vidriopanel` DISABLE KEYS */;
/*!40000 ALTER TABLE `proyecto_vidriopanel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proyecto_vp`
--

DROP TABLE IF EXISTS `proyecto_vp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proyecto_vp` (
  `Id_Subcomponente` int NOT NULL,
  `Altura` int NOT NULL,
  `Anchura` int NOT NULL,
  `Cantidad` int NOT NULL,
  `Id_Unidad_Medida` int NOT NULL,
  `medida` decimal(18,2) NOT NULL,
  `medidaAdicional` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proyecto_vp`
--

LOCK TABLES `proyecto_vp` WRITE;
/*!40000 ALTER TABLE `proyecto_vp` DISABLE KEYS */;
/*!40000 ALTER TABLE `proyecto_vp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subcomponentes`
--

DROP TABLE IF EXISTS `subcomponentes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `subcomponentes` (
  `Id_Subcomponente` int NOT NULL AUTO_INCREMENT,
  `Id_Acabado` int NOT NULL,
  `Codigo_Homologacion` varchar(20) NOT NULL,
  `Descripcion` varchar(200) NOT NULL,
  `Especial` tinyint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id_Subcomponente`),
  UNIQUE KEY `Codigo_UNIQUE` (`Codigo_Homologacion`)
) ENGINE=InnoDB AUTO_INCREMENT=376 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subcomponentes`
--

LOCK TABLES `subcomponentes` WRITE;
/*!40000 ALTER TABLE `subcomponentes` DISABLE KEYS */;
INSERT INTO `subcomponentes` VALUES (9,7,'VIC0001','PERFIL ZOCALO VITRUM ',1),(10,7,'VIC0002','PERFIL TAPA VITRUM',0),(11,7,'VIC0003','PERFIL MODULO TECNICO',0),(12,7,'VIC0004','PERFIL TAPA MODULO TECNICO',0),(13,8,'VIC0005','NIVELADOR ',0),(14,6,'VIC0006','UNION PUERTA ZOCALO ',0),(15,8,'VIC0007','SOPORTE TAPA ',0),(16,2,'VIC0008','PERFIL TAPA PINZA ',0),(17,12,'VIC0009','PERFIL BASE PINZA',0),(28,5,'VIC0010','TUERCA EXCENTRICA PINZA ',0),(29,6,'VIC0011','SOPORTE NIVELADOR BLUEBRANCH ',0),(31,6,'VIC0012','TORNILLO NIVELADOR BLUEBRANCH M6x16 ',0),(32,8,'VIC0013','TAPON TORNILLO NIVELADOR BLUEBRANCH ',0),(33,9,'VIC0014','TAPON FINAL BLUEBRANCH ',0),(34,9,'VIC0015','TOPE INFERIOR IZQUIERDO BLUEBRANCH ',0),(35,9,'VIC0016','TOPE INFERIOR DERECHO BLUEBRANCH ',0),(36,9,'VIC0017','TOPE SUPERIOR IZQUIERDO BLUEBRANCH ',0),(37,9,'VIC0018','TOPE SUPERIOR DERECHO BLUEBRANCH ',0),(38,6,'VIC0019','TORNILLO SOPORTE DE TOPE/TAPON BLUEBRANCH M4x20 ',0),(39,8,'VIC0020','ARANDELA TOPE DE COSTILLA BLUEBRANCH ',0),(40,7,'VIC0021','TOPE PARED BLUEBRANCH ',0),(41,7,'VIC0022','GOMA VITRUM 5+5 ',0),(42,7,'VIC0023','GOMA VITRUM 6+6 ',0),(43,7,'VIC0024','GOMA VITRUM FIJA ',0),(44,12,'VIC0025','CINTA ADHESIVA DOBLE CARA 2mm 5+5 (18mx6mm) ',0),(45,12,'VIC0026','CINTA ADHESIVA DOBLE CARA 2mm 5+5 (15mx6mm) ',0),(46,12,'VIC0027','CINTA ADHESIVA DOBLE CARA 1mm 5+5 (25mx6mm) ',0),(47,12,'VIC0028','CINTA ADHESIVA DOBLE CARA 2mm 6+6 (18mx9mm) ',0),(48,7,'VIC0029','TUBO 40x40 ',0),(49,5,'VIC0030','ESCUADRA UNION MODULO TECNICO ',0),(50,19,'VIC0031','BURLETE ANCHO GRIS ANRACITA 25mx17mmx2mm ',0),(51,18,'VIC0032','BURLETE ANCHO GRIS CLARO 25mx17mmx2mm ',0),(52,17,'VIC0033','CINTA ADHESIVA DOBLE CARA FINA (50mx19mmx0,1mm) ',0),(53,13,'VIC0034','TORNILLO UNION PTA./ZOC. M6x30 ',0),(54,15,'VIC0035','ANGULO 50 x 50 x 6mm ',0),(55,7,'VIC0100','PERFIL MARCO PUERTA VITRUM ',0),(56,7,'VIC0106','GOMA DE BUBUJA PUERTA VITRUM ',0),(57,7,'VIC0107','GOMA DE MARCO VITRUM PARA 5+5 ',0),(58,7,'VIC0108','U  15x14.5x15 GUIA MARCO PUERTA L=3000 ',0),(59,12,'VIC0110','TUBO 25x15 ',0),(60,7,'VIC0111','T 40 x 40 ',0),(61,6,'VIC0500','ESCUADRA MARCO VITRUM ',0),(62,5,'VIC0501','BISAGRA BLUEBRANCH IZQUIERDA VIDRIO-VIDRIO ',0),(63,5,'VIC0502','BISAGRA BLUEBRANCH DERECHA VIDRIO-VIDRIO ',0),(64,5,'VIC0503','BISAGRA BLUEBRANCH IZQUIERDA FRENO VIDRIO-VIDRIO ',0),(65,5,'VIC0504','BISAGRA BLUEBRANCH DERECHA FRENO VIDRIO-VIDRIO ',0),(66,5,'VIC0505','BISAGRA BLUEBRANCH IZQUIERDA PARED-VIDRIO ',0),(67,5,'VIC0506','BISAGRA BLUEBRANCH DERECHA PARED-VIDRIO ',0),(68,5,'VIC0507','BISAGRA BLUEBRANCH IZQUIERDA FRENO PARED-VIDRIO ',0),(69,5,'VIC0508','BISAGRA BLUEBRANCH DERECHA FRENO PARED-VIDRIO ',0),(70,13,'VIC0509','TIRADOR PUERTA ',0),(71,13,'VIC0510','TOPE PUERTA DOBLE BLUEBRANCH ',0),(72,6,'VIC0511','TORNILLO EXCENTRICA BASE PINZA M5x10 ',0),(73,6,'VIC0512','ESPARRAGO TAPA PINZA M5x6 ',0),(74,15,'VIC0513','ESCUADRA PIVOTANTE MARCO VITRUM ',0),(75,5,'IMC0001','PERFIL U ACERO TECHO 60x20 ',0),(76,5,'IMC0002','PERFIL ACERO VERTICAL DE 40 ',0),(77,5,'IMC0003','PERFIL ACERO VERTICAL DE 20 ',0),(78,1,'IMC0004','PERFIL ZOCALO OX ACABADO LACADO NEGRO',0),(79,7,'IMC0005','PERFIL 100x100',0),(80,7,'IMC0006','PERFIL 100x20 ',0),(87,7,'IMC0007','PERFIL MONOVIDRIO RECTO ',0),(88,7,'IMC0008','GOMA IMPULS 6+6 ',0),(89,12,'IMC0010','CINTA ADHESIVA DOBLE CARA 2mm 3+3 (16,5mx4mm) ',0),(90,14,'IMC0011','CALCE VIDRIO VERDE 2mm  ',0),(91,15,'IMC0012','CALCE VIDRIO AZUL 3mm  ',0),(92,10,'IMC0013','CALCE VIDRIO NEGRO 4mm ',0),(93,8,'IMC0014','CALCE VIDRIO BLANCO 5mm ',0),(94,7,'IMC0015','TAPA PERFIL VIDRIO DOBLE ',0),(95,7,'IMC0016','TAPA LARGA PERFIL VIDRIO DOBLE ',0),(96,19,'IMC0017','BURLETE ESTRECHO GRIS ANRACITA 25mx9mmx2mm ',0),(97,18,'IMC0018','BURLETE ESTRECHO GRIS CLARO 25mx9mmx2mm ',0),(98,21,'IMC0019','BURLETE TELA NEGRO 50mx10mm ',0),(99,20,'IMC0020','BURLETE TELA GRIS 50mx10mm ',0),(100,19,'IMC0021','BURLETE ANCHO TABLEROS GRIS ANRACITA 25mx15mmx3mm',0),(101,15,'IMC0022','ANGULO 40 x 40 x 4mm ',0),(102,15,'IMC0023','PLETINA 40 x 8mm ',0),(103,15,'IMC0024','PLETINA 40 x 4mm ',0),(104,7,'IMC0100','PERFIL MARCO PUERTA DV ',0),(105,7,'IMC0101','GOMA DE BUBUJA PUERTA IMPULS ',0),(106,7,'IMC0102','PERFIL TAPA MARCO PUERTA 100 ',0),(107,7,'IMC0103','MARCO DE PUERTA ACÚSTICA DE 100 ',0),(108,7,'IMC0104','BASTIDOR HOJA DE PUERTA ACÚSTICA DE 100 ',0),(109,7,'IMC0105','BASTIDOR HOJA DE PUERTA ACÚSTICA DE 38 ',0),(110,7,'IMC0106','BASTIDOR HOJA DE PUERTA VIDRIO SIMPLE DE 38 ',0),(111,7,'IMC0107','U PLASTICO VIDRIO 6mm BASTIDOR PUERTA VIDRIO SIMPLE DE 38',0),(115,7,'IMC0108','L 60 x 15 ',0),(116,7,'IMC0109','L 40 x 20 ',0),(117,5,'IMC0500','PERFIL TELESCOPICO ACERO DE 40 ',0),(118,5,'IMC0501','PERFIL TELESCOPICO ACERO DE 20 ',0),(119,6,'IMC0502','NIVELADOR MONTANTE CIEGO ',0),(120,6,'IMC0503','\"SOPORTE DE AJUSTE PERFIL VERTICAL LIGERO\" ',0),(121,6,'IMC0504','SOPORTE DE AJUSTE PERFIL VERTICAL ',0),(122,7,'IMC0505','ANGULAR DE SOPORTE LARGUERO ',0),(123,6,'IMC0506','ANGULAR DE SOPORTE LARGUERO ATORNILLA ',0),(124,6,'IMC0507','REGULADOR TELESCOPICO ',0),(125,6,'IMC0508','NUEVO CLIP DE COLGAR ',0),(126,5,'IMC0509','ESCUADRA SOPORTE 100x100-100x20-MARCO FLOT ',0),(132,15,'IMC0510','ESCUADRA MARCO DV ',0),(133,15,'IMC0511','ESCUADRA MARCO 100 ',0),(136,6,'IMC0512',' ESCUADRA BASTIDOR PTA. ACUSTICA 38 ',0),(137,15,'IMC0513','ESCUADRA BASTIDOR PTA. ACUSTICA 100 ',0),(138,15,'IMC0514','ESCUADRA VIDRIO DOBLE ',0),(139,15,'IMC0515','HEMBRA TENSOR PTA. ACUSTICA 38 M4x20 ',0),(140,15,'IMC0516','VARILLA TENSOR PTA.ACUSTICA 38 M4 ',0),(141,15,'IMC0517','TORNILLO TENSOR PTA. ACUSTICA 38 M4x30 ',0),(142,7,'IMC0518','TAPA TORNILLO PTA. ACUSTICA 100 ',0),(143,5,'IMC0519','CENTRADOR PANEL ',0),(144,29,'IMC0520','LISTON BASTIDOR PTA. ACUSTICA 38 ',0),(145,15,'IMC0521','ESCUADRA PIVOTANTE MARCO DV IZQ + PLETINA INFERIOR SUJECCION ',0),(146,15,'IMC0522','ESCUADRA PIVOTANTE MARCO DV DCH + PLETINA INFERIOR SUJECCION ',0),(147,15,'IMC0523','ESCUADRA INTERIOR BASTIDOR ACUSTICA 38 40x40x4 (ANTIGUA) ',0),(148,7,'ICC0001','PERFIL ZOCALO BAJO ',0),(149,7,'ICC0002','PERFIL ZOCALO ALTO ',0),(150,16,'ICC0003','LANA DE ROCA ',0),(151,7,'DVC0001','PERFIL VIDRIO DOBLE ',0),(152,7,'DVC0002','IMPULS VIDRIO DOBLE ABIERTO ',0),(153,7,'DVC0003','IMPULS TAPA VIDRIO DOBLE ABIERTO ',0),(154,7,'DVC0004','PERFIL VENTANA VIDRIO DOBLE ',0),(155,7,'AVC0001','PERFIL AV ',0),(156,7,'AVC0002','PERFIL U IMPULS ',0),(157,7,'AVC0003','TAPON AV ',0),(158,7,'AVC0100','PERFIL MARCO PUERTA AV ',0),(159,15,'IMC0110','ESCUADRA MARCO AV ',0),(160,15,'IMC0111','ESCUADRA PIVOTANTE MARCO AV ',0),(161,1,'ITC0001','PERFIL IMPULS T ACABADO LACADO NEGRO 9005 MATE LISO',0),(162,7,'ITC0002','PERFIL VIDRIO DOBLE IMPULS T ',0),(163,7,'ITC0100','MARCO PUERTA IMPULS T ',0),(164,7,'MTC0001','MARCO TELESCOPICO FIJO ',0),(165,7,'MTC0002','MARCO TELESCOPICO MOVIL ESTRECHO ',0),(167,7,'MTC0003','MARCO TELESCOPICO MOVIL ANCHO ',0),(168,10,'MTC0004','GOMA MARCO TELESCOPICO ',0),(169,15,'MTC0501','ESCUADRA PLANA MARCO TELESCOPICO ',0),(170,5,'PHC0005','ESCUADRA UNION ACERO ',0),(171,5,'PHC0006','ESCUADRA ESTRUCTURA ',0),(172,1,'PHC0501','CHAPA PLEGADA SUELO ',0),(173,5,'PHC0502','L UNION BALDA ',0),(174,1,'PHC0503','REJILLA RECTANGULAR ',0),(175,1,'PHC0504','REJILLA CUADRADA ',0),(176,7,'PHC0505','MOQUETA ',0),(177,7,'PHC0506','VENTILADOR BEQUIET ',0),(178,7,'PHC0507','TRANSFORMADOR PEQUEÑO ',0),(179,7,'PHC0508','TRANSFROMADOR GRANDE ',0),(180,7,'PHC0509','DETECTOR DE LUZ ',0),(181,7,'PHC0510','CAJAS REGISTRABLES ',0),(182,7,'PHC0511','BUZZY ',0),(183,7,'PHC0512','KAPSA XS 1T+1 USB CARGADOR ',0),(184,7,'PHC0513','MACHO WIELAND ',0),(185,7,'PHC0514','HEMBRA WIELAND ',0),(186,7,'PHC0515','MULTIPLICADOR WIELAND x3 ',0),(187,1,'PHC0516','KAPSA XS TAPA NEGRA ',0),(188,7,'PHC0517','CABLE SCHUKO Y WIELAND HEMBRA ',0),(189,7,'PHC0518','TIRA LED ',0),(190,5,'PHC0519','ZOCALO PARA EL LED ',0),(191,7,'PHC0520','TAPA PARA EL LED ',0),(192,7,'PHC0601','VIDRIO TEMPLADO 10mm PUERTA ',0),(193,7,'PHC0602','VIDRIO LAMINAR 5+5 SILENCE ',0),(194,2,'HEC0500','CERRADURA ARMARIO EXENTO ',0),(195,2,'HEC0501','CERRADURA CAJON METALICO ',0),(196,2,'HEC0502','CERRADURA FALLEBA ',0),(197,9,'HEC0503','CERRADURA COMBINACION DERECHA LOCK 57 ',0),(198,10,'HEC0504','CERRADURA COMBINACION IZQUIERDA LOCK 57 ',0),(199,10,'HEC0505','CERRADURA COMBINACION DERECHA LOCK 59 ',0),(205,14,'HEC0506','LLAVE EXTRACTORA BOMBILLOS ',0),(206,14,'HEC0507','PINTXO CERRADURA COMBINACION',0),(207,2,'HEC0508','TIRADOR POMO ',0),(208,2,'HEC0509','TIRADOR  \" L \" ',0),(209,8,'HEC0510','TIRADOR PUSH ',0),(210,10,'HEC0511','TIRADOR PUSH ',0),(211,2,'HEC0512','TIRADOR CONCHA ENCASTRADO ',0),(212,2,'HEC0513','POMOS DERECHA ',0),(213,14,'HEC0514','CAJONES METÁLICOS ',0),(214,10,'HEC0515','KEKU MACHO ',0),(215,10,'HEC0516','KEKU HEMBRA ',0),(216,6,'HEC0517','NIVELADOR ',0),(217,28,'HEC0518','GALLETAS ',0),(218,6,'HEC0519','EXCENTRICA 15x15 ',0),(219,6,'HEC0520','TORNILLO EXCENTRICA  ',0),(220,28,'HEC0521','ESPIGA D8 ',0),(221,7,'HEC0522','VELCRO ',0),(222,6,'HEC0523','TUERCA EMBUTIDA ',0),(223,6,'HEC0524','TORNILLO M8x20 ',0),(224,6,'HEC0525','ARANDELA DENTADA PARA M8 ',0),(225,6,'HEC0526','PUNTABROCA Ø4,2x16 ',0),(226,6,'HEC0527','PUNTABROCA 4,2x17 ',0),(227,6,'HEC0528','TIRAFONDO 4,2x18 ',0),(228,2,'HEC0529','PERCHA EXTRAIBLE L45 ',0),(229,2,'HEC0530','PERCHA EXTRAIBLE L40 ',0),(230,2,'HEC0531','PERCHA EXTRAIBLE L35 ',0),(231,10,'HEC0532','RUEDAS CAJONERAS ',0),(232,2,'HEC0533','ESCUADRA CERRADURA ',0),(233,2,'HEC0534','LENGÜETA CERRADURA ',0),(234,10,'HEC0535','PATA H17 ',0),(235,10,'HEC0536','PATA H45 ',0),(236,2,'HEC0537','SOPORTE MOD. COLGAR OFFICE ',0),(237,2,'HEC0538','CHAPA MOD. COLGAR OFFICE ',0),(238,10,'HEC0539','PATA MOD. BAJOS OFFICE ',0),(239,10,'HEC0540','GRAPA PATA MOD. BAJOS OFFICE ',0),(240,9,'HEC0541','CAZOLETA D20 ',0),(241,2,'HEC0542','TORNILLO PARA CAZOLETA D20 ',0),(242,2,'HEC0543','CAZOLETA D15 ',0),(243,2,'HEC0544','TORNILLO L24 PARA CAZOLETA D15 ',0),(244,2,'HEC0545','CAZOLETA D20 ',0),(245,2,'HEC0546','TORNILLO PARA CAZOLETA D20 ',0),(246,14,'HEC0547','SET CAJON 70/420 H30 GRIS PLATA ',0),(247,2,'HEC0548','NIVELADOR ARM.TABIQUE ',0),(248,12,'HEC0549','ESCUADRA PARA NIVELADOR ARM.TABIQUE ',0),(249,8,'HEC0550','TAPON PARA NIVELADOR ARM.TABIQUE ',0),(250,2,'HEC0551','ESCUADRA TRASERA ',0),(251,2,'HEC0552','TORNILLO ESCUADRA TRASERA ',0),(252,10,'HEC0553','MEDIA LUNA - CERRADURA ',0),(263,2,'HEC0555','BISAGRA RECTA ',0),(264,2,'HEC0556','BISAGRA OCULTA ',0),(265,2,'HEC0557','BISAGRA RECTA AMORTIGUADA ',0),(266,6,'HEC0580','BISAGRA PUERTA CIEGA RF 1076 R10 2BB INOX ',0),(267,6,'HEC0581','CERRADURA PUERTA CIEGA ',0),(269,6,'HEC0582','JUEGO MANILLA 90 º RECTA PUERTA CIEGA ',0),(270,6,'HEC0583','JUEGO ESCUDO BOCALLAVE PUERTA CIEGA ',0),(271,7,'HEC0584','CERRADURA PUERTA VIDRIO DORMA ',0),(272,7,'HEC0585','CERRADURA PUERTA VIDRIO DORMA PASO ',0),(273,7,'HEC0586','CERRADERO PUERTA DOBLE VIDRIO DORMA ',0),(274,7,'HEC0587','MANILLA PUERTA DE VIDRIO DORMA ',0),(275,7,'HEC0588','POMO/MANILLA PUERTA DE VIDRIO DORMA ',0),(276,6,'HEC0589','BOMBILLO 60 (30/30) LEVA LARGA ',0),(277,7,'HEC0590','PERFIL PIVOTANTE ',0),(278,7,'HEC0591','BISAGRA INFERIOR IZQUIERDA PIVOTANTE VITRUM/DV ',0),(279,7,'HEC0592','BISAGRA SUPERIOR IZQUIERDA PIVOTANTE VITRUM/DV ',0),(280,7,'HEC0593','BISAGRA INFERIOR DERECHA PIVOTANTE VITRUM/DV ',0),(281,7,'HEC0594','BISAGRA SUPERIOR DERECHA PIVOTANTE VITRUM/DV ',0),(282,7,'HEC0595','BISAGRA INFERIOR IZQUIERDA PIVOTANTE AV ',0),(283,7,'HEC0596','BISAGRA SUPERIOR IZQUIERDA PIVOTANTE AV ',0),(284,7,'HEC0597','BISAGRA INFERIOR DERECHA PIVOTANTE AV ',0),(285,7,'HEC0598','BISAGRA SUPERIOR DERECHA PIVOTANTE AV ',0),(286,8,'HEC0599','TAPON PERFIL PIVOTANTE ',0),(287,8,'HEC0600','ARANDELA PIVOTANTE ',0),(288,8,'HEC0601','CASQUILLO PERFIL PIVOTANTE ',0),(289,6,'HEC0602','ESPARRAGO CASQUILLO PERFIL PIVOTANTE M5x25 ',0),(290,7,'HEC0603','BISAGRA PUERTA DE VIDRIO DORMA ',0),(291,7,'HEC0604','BISAGRA PUERTA DE VIDRIO MINUSCO ',0),(292,5,'HEC0605','PLACA SOPORTE BISAGRA GRANDE   LA PAZ ',0),(293,5,'HEC0606','PLACA SOPORTE BISAGRA JNF ',0),(294,13,'HEC0607','TORNILLO PLACA BISAGRA DIN 7991 M4 x 10 INOX ',0),(295,13,'HEC0608','TOPE PUERTA ',0),(296,6,'HEC0609','TORNILLO BISAGRA PIVOTANTE ',0),(297,13,'HEC0610','JUEGO ESCUDO JNF PUERTA ACUSTICA PASO JUEGO ESCUDO JNF PUERTA ACUSTICA PASO ',0),(298,13,'HEC0611','JUEGO ESCUDO JNF PUERTA ACUSTICA CON BOMBILLO ',0),(299,13,'HEC0612','JUEGO MANILLA JNF SIN ROSETA ',0),(300,6,'HEC0613','CERRADURA JNF 50 MECANICA PUERTA ACUSTICA 40 ',0),(301,6,'HEC0614','CERRADURA JNF 60 MECANICA PUERTA ACUSTICA 100 ',0),(302,7,'HEC0615','BISAGRA OCULTA KUBIK ',0),(303,6,'HEC0616','GUILLOTINA PUERTA ACUSTICA 100 ',0),(304,7,'HEC0617','CHAPA GUILLOTINA ',0),(305,6,'HEC0618','CUADRADILLO PUERTA ACUSTICA 100 ',0),(306,6,'HEC0619','BOMBILLO 120 (90/30) LEVA LARGA ',0),(307,27,'HEC0620','CALCE DM CERRADURA ',0),(308,6,'HEC0621','CIERRE ELÉCTRICO ',0),(309,6,'HEC0622','CUADRADILLO MEDIO LOCO ',0),(310,13,'HEC0623','POMO/MANILLA PUERTA CIEGA ',0),(311,7,'HEC0624','BISAGRA PIVOTANTE DERECHA PUERTA CIEGA ',0),(312,7,'HEC0625','BISAGRA PIVOTANTE IZQUIERDA PUERTA CIEGA ',0),(313,7,'HEC0626','BISAGRA PIVOTANTE DERECHA PUERTA V.S. ENMARCADA ',0),(314,7,'HEC0627','BISAGRA PIVOTANTE IZQUIERDA PUERTA V.S. ENMARCADA ',0),(315,7,'HEC0628','ESCUDO MANILLA PUERTA MINIMALISTA ',0),(316,7,'HEC0629','CERRADURA PUERTA ENMARCADA VIDRIO SIMPLE DE 38 ',0),(317,7,'HEC0630','EMBELLECEDOR CERRADERO ',0),(318,27,'HEC0631','TACO DM CERRADURA PUERTA ENMARCADA VIDRIO SIMPLE DE 38 ',0),(319,7,'HEC0632','MUELLE DORMA TS-90 ',0),(324,7,'HEC0633','MUELLE DORMA TS-92 ',0),(325,7,'HEC0634','GUANTE VIDRIO MUELLE DORMA    TS-92 ',0),(326,14,'HEC0635','TORNILLO TACO Ø3,9x45 ',0),(327,8,'HEC0636','TACO TORNILLO Ø6x35 ',0),(328,6,'HEC0637','PUNTABROCA Ø3,5x13 ',0),(329,6,'HEC0638','PUNTABROCA Ø3,5x19 ',0),(330,6,'HEC0639','PUNTABROCA Ø4,2x22 ',0),(331,6,'HEC0640','PUNTABROCA Ø4,2x25 ',0),(332,6,'HEC0641','PUNTABROCA Ø4,2x38 ',0),(333,6,'HEC0642','PUNTABROCA Ø4,8x25 ',0),(334,6,'HEC0643','PUNTABROCA Ø4,8x38 ',0),(339,6,'HEC0644','ROSCACHAPA CABEZA EXTRAPLANA Ø4,2x22 ',0),(340,6,'HEC0645','ROSCACHAPA CABEZA CILINDRICA Ø4,2x16 ',0),(341,6,'HEC0646','TIRAFONDO ROSCAMADERA Ø3x16 ',0),(342,6,'HEC0647','TIRAFONDO ROSCAMADERA Ø3x20 ',0),(343,6,'HEC0648','TIRAFONDO ROSCAMADERA Ø3x25 ',0),(344,6,'HEC0649','TIRAFONDO ROSCAMADERA Ø4x16 ',0),(345,6,'HEC0650','TIRAFONDO ROSCAMADERA Ø4x20 ',0),(346,6,'HEC0651','TIRAFONDO ROSCAMADERA Ø4x25 ',0),(347,6,'HEC0652','TIRAFONDO ROSCAMADERA Ø5x30 ',0),(348,6,'HEC0653','TIRAFONDO ROSCAMADERA Ø5x40 ',0),(349,6,'HEC0654','TIRAFONDO ROSCAMADERA Ø5x50 ',0),(350,6,'HEC0655','TIRAFONDO ROSCAMADERA Ø5x60 ',0),(351,13,'HEC0656','TIRAFONDO ROSCAMADERA Ø4,5x25 ',0),(352,13,'HEC0657','TIRAFONDO ROSCAMADERA Ø4,5x40 ',0),(353,24,'HEC0670','TACOLIT ',0),(354,24,'HEC0671','SICA ',0),(355,24,'HEC0672','ADHESIVO MS POLIMERO ',0),(360,23,'HEC0673','SILICONA TRANSPARENTE ',0),(361,7,'HEC0680','CERRADILLO PTA. DOBLE VIDRIO ',0),(362,6,'HEC0681','PESTILLO PUERTA DOBLE CIEGA CORTO 200mm ',0),(363,6,'HEC0682','PESTILLO PUERTA DOBLE CIEGA LARGO 500mm ',0),(364,7,'HEC0683','PESTILLO SOBREPONER PUERTA ENMARCADA VIDRIO ',0),(368,1,'0001','prueba 1',1),(369,1,'0002','prueba 2',1),(370,2,'VID5+5','VIDRIO 5+5MM ACABADO NATURAL',1),(371,1,'tub','tubo metalico 100',0),(373,25,'VID6+6','VIDRIO 6+6MM TRANSPARENTE',1),(374,7,'PREM','PERFIL DE REMATE 100 X 20',0);
/*!40000 ALTER TABLE `subcomponentes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tbauxanchura`
--

DROP TABLE IF EXISTS `tbauxanchura`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tbauxanchura` (
  `Altura` varchar(10) DEFAULT NULL,
  `Columna1` varchar(10) DEFAULT NULL,
  `Columna2` varchar(10) DEFAULT NULL,
  `Columna3` varchar(10) DEFAULT NULL,
  `Columna4` varchar(10) DEFAULT NULL,
  `Columna5` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tbauxanchura`
--

LOCK TABLES `tbauxanchura` WRITE;
/*!40000 ALTER TABLE `tbauxanchura` DISABLE KEYS */;
INSERT INTO `tbauxanchura` VALUES ('1872','1082','0','0','0','0'),('1872','1082','0','0','0','0'),('1872','1082','0','0','0','0'),('2969','846','0','0','0','0'),('2969','846','0','0','0','0'),('2969','862','0','0','0','0'),('2969','862','0','0','0','0'),('2969','712','0','0','0','0'),('2969','1186','0','0','0','0'),('2969','1186','0','0','0','0'),('2969','1182','0','0','0','0'),('2969','1182','0','0','0','0'),('2969','995','0','0','0','0'),('2969','995','0','0','0','0'),('2969','995','0','0','0','0'),('2969','995','0','0','0','0'),('2969','970','0','0','0','0'),('2969','970','0','0','0','0');
/*!40000 ALTER TABLE `tbauxanchura` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unidades_calculadas`
--

DROP TABLE IF EXISTS `unidades_calculadas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unidades_calculadas` (
  `Id_Unidad_Calculada` int NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Estado` int DEFAULT NULL,
  PRIMARY KEY (`Id_Unidad_Calculada`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unidades_calculadas`
--

LOCK TABLES `unidades_calculadas` WRITE;
/*!40000 ALTER TABLE `unidades_calculadas` DISABLE KEYS */;
INSERT INTO `unidades_calculadas` VALUES (1,'Longitud Recopilatoria',1),(2,'Unidad',1),(3,'Cantidad',1),(4,'Medida Exacta',1),(5,'Longitud sin Recopilar',1);
/*!40000 ALTER TABLE `unidades_calculadas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unidades_medidas`
--

DROP TABLE IF EXISTS `unidades_medidas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unidades_medidas` (
  `Id_Unidad_Medida` int NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(50) NOT NULL,
  `Convencion` varchar(10) NOT NULL,
  PRIMARY KEY (`Id_Unidad_Medida`),
  UNIQUE KEY `Descripcion_UNIQUE` (`Descripcion`),
  UNIQUE KEY `Convencion_UNIQUE` (`Convencion`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unidades_medidas`
--

LOCK TABLES `unidades_medidas` WRITE;
/*!40000 ALTER TABLE `unidades_medidas` DISABLE KEYS */;
INSERT INTO `unidades_medidas` VALUES (1,'Kilogramos','kg'),(2,'Unidad','ud'),(3,'Metros','m'),(4,'Metros Cuadrado','m²');
/*!40000 ALTER TABLE `unidades_medidas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `contrasena` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Nombre` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `estado` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'admin','admin08','Administrador',1);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'arquitectdb'
--
/*!50003 DROP FUNCTION IF EXISTS `fnUnidadCalculada` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fnUnidadCalculada`(
 idUnidadCalculada int
) RETURNS varchar(100) CHARSET utf8
BEGIN
   DECLARE unidad nvarchar(100); 
  
 SET unidad = "";
 
  IF idUnidadCalculada = 1 THEN 
    SET unidad = "Longitud Recompilacón";
  ELSEIF idUnidadCalculada = 2 THEN
    SET unidad = "Unidad";
  ELSEIF idUnidadCalculada = 3 THEN
    SET unidad = "Cantidad";
   ELSEIF idUnidadCalculada = 4 THEN
    SET unidad = "Medida Exacta";
   ELSEIF idUnidadCalculada = 5 THEN
    SET unidad = "Longitud sin Recompilacón";
  END IF;
  
 RETURN unidad;
 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `fnUnidadCalculadaVidrioPanel` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fnUnidadCalculadaVidrioPanel`(
idUnidadCalculada int
) RETURNS varchar(100) CHARSET utf8
BEGIN
 
  DECLARE unidad nvarchar(100);
  
 SET unidad = "";
 
 -- SET GLOBAL log_bin_trust_function_creators = 1;
 
		  IF idUnidadCalculada = 1 THEN 
			 SET unidad = "1|Columna primera";
		    ELSEIF idUnidadCalculada = 2 THEN
			 SET unidad = "2|Columna segunda";
		    ELSEIF idUnidadCalculada = 3 THEN
			 SET unidad = "3|Columna tercera";
		    ELSEIF idUnidadCalculada = 4 THEN
			 SET unidad = "4|Columna cuarta";
		    ELSEIF idUnidadCalculada = 5 THEN
			 SET unidad = "5|Columna quinta";
		    ELSEIF idUnidadCalculada = 6 THEN
			 SET unidad = "6|Longitud";
		    ELSEIF idUnidadCalculada = 7 THEN
			 SET unidad = "7|Unidad";
		  END IF;
  
 RETURN unidad;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componenteDetalleCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componenteDetalleCargar`(
idComponente int
)
BEGIN
   SELECT detalle.Id_Subcomponente,  
		   CONCAT(subcomponente.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion) codigo,
		   subcomponente.Descripcion Descripcion_Ori,
           CONCAT(subcomponente.Descripcion , " (" , acabados.Descripcion,") ") Descripcion,
           CONCAT(detalle.Id_Unidad_Calculada, "|", fnUnidadCalculada(detalle.Id_Unidad_Calculada)) unidad,
           detalle.Id_Unidad_Calculada,
		   Cantidad_Default ,
		   Cantidad_Adicional ,
		   Aplica_Decremento,
            detalle.elevado,
            CASE WHEN corte.Id_Corte IS NULL THEN 0 ELSE corte.Id_Corte END AS corte
	FROM arquitectdb.componentes_detalle detalle 
	JOIN arquitectdb.subcomponentes subcomponente ON detalle.Id_Subcomponente = subcomponente.Id_Subcomponente
	JOIN arquitectdb.acabados ON subcomponente.Id_Acabado = acabados.Id_Acabado
     LEFT JOIN arquitectdb.cortes corte ON corte.Id_Corte = detalle.idCorte
	WHERE Id_Componente = idComponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componenteEspecialDetalleCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componenteEspecialDetalleCargar`(
  idComponente int
)
BEGIN
       SELECT detalle.Id_Subcomponente,  
		   CONCAT(subcomponente.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion) codigo,
		   subcomponente.Descripcion,
            fnUnidadCalculadaVidrioPanel(detalle.select_Columna) select_Columna,
            detalle.select_Columna Id_Columna,	
            Cantidad_Default ,
		    Cantidad_Adicional ,
		    Aplica_Decremento,
            detalle.elevado,
            CASE WHEN corte.Descripcion IS NULL THEN '-- Seleccionar --' ELSE corte.Descripcion END AS corte
	FROM arquitectdb.componentes_Especial_detalle detalle 
	JOIN arquitectdb.subcomponentes subcomponente ON detalle.Id_Subcomponente = subcomponente.Id_Subcomponente
	JOIN arquitectdb.acabados ON subcomponente.Id_Acabado = acabados.Id_Acabado
    LEFT JOIN arquitectdb.cortes corte ON corte.Id_Corte = detalle.idCorte
	WHERE Id_Componente_Especial = idComponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentesConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentesConsultar`(
  pCadena nvarchar(100)
)
BEGIN
    SELECT Id_Componente,Codigo,Descripcion 
    FROM arquitectdb.componentes
    WHERE CONCAT(Codigo , "-" , Descripcion) lIKE concat('%',  pCadena , '%');
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentesEspecialConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentesEspecialConsultar`(
  pCadena nvarchar(100)
)
BEGIN
      SELECT Id_Componente_Especial,Codigo,Descripcion 
        FROM arquitectdb.componentes_Especial
        WHERE CONCAT(Codigo , "-" , Descripcion) lIKE concat('%',  pCadena , '%');
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteActualizar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteActualizar`(
 pCodigo nvarchar(50),
 pDescripcion nvarchar(300),
 pNoSubcomponente boolean,
 pIdComponente int
)
BEGIN
   UPDATE arquitectdb.componentes SET codigo = pCodigo, 
                                       descripcion = pDescripcion,
                                       NoSubcomponente = pNoSubcomponente
	WHERE Id_Componente = pIdComponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteCargar`(
 pidComponente Int,
 plogitud      Int,
 panchura      Int,
 paltura       Int ,
 parea         Int
)
BEGIN

    DECLARE idUnidadCalculada INT; 
    DECLARE cantidad DECIMAL;
	DECLARE aplicaDecremento BIT;
            
    SET @idUnidadCalculada = (SELECT id_Unidad_calculada FROM subcomponentes where id_subcomponente = pidComponente);
    
    IF (@idUnidadCalculada = 1) THEN 
    /* LOGITUD */   
    
       SET @aplicaDecremento = (SELECT aplica_decremento FROM subcomponentes where id_subcomponente = pidComponente);
       
       IF  (@aplicaDecremento = 0) THEN 
	     SET @medida = (SELECT cantidad_adicional FROM subcomponentes where id_subcomponente = pidComponente) + plogitud;
       ELSE 
	     SET @medida = (SELECT cantidad_adicional FROM subcomponentes where id_subcomponente = pidComponente) - plogitud;
       END IF;
       
      SELECT id_subcomponente,
	         codigo_homologacion codigo,
             descripcion,
             Cantidad_defaultd cantidad, 
             @medida medida
      FROM  subcomponentes where id_subcomponente = pidComponente;
 
    ELSEIF (@idUnidadCalculada = 2) THEN 
    /* UNIDAD */   
     SET @cantidad = plogitud/1000;
     
         SELECT  id_subcomponente,
	              codigo_homologacion codigo,
                  descripcion,
				  CEILING(@cantidad) cantidad, 
                  plogitud 
		 FROM subcomponentes where id_subcomponente = pidComponente;
   ELSE
		SELECT  id_subcomponente,
	              codigo_homologacion codigo,
                  descripcion,
				  Cantidad_defaultd, 
                  plogitud 
		 FROM subcomponentes where id_subcomponente = pidComponente;
    END IF;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteEspecialActualizar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteEspecialActualizar`(
   pCodigo nvarchar(50),
   pDescripcion nvarchar(300),
   pIdComponente int
)
BEGIN
    UPDATE arquitectdb.componentes_Especial SET codigo = pCodigo, 
												descripcion = pDescripcion
	WHERE Id_Componente_Especial = pIdComponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponentePerfilesCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponentePerfilesCargar`(
 pCodigo       nvarchar(50),
 plogitud      float
 
)
BEGIN

    DECLARE idUnidadCalculada INT; 
	DECLARE aplicaDecremento BIT;
    DECLARE idv Int;
    DECLARE idSubComponentev Int;
    DECLARE count INT;
    DECLARE contador INT default 0;
    DECLARE cantidad_U Int;
    DECLARE pmedida float;
    
    SET SQL_SAFE_UPDATES = 0;
    
	CREATE TEMPORARY TABLE tableResult (id_subcomponente Int, 
										id_unidad_Calculada Int,
                                        codigo nvarchar(10), 
                                        descripcion nvarchar(200),
                                        cantidad Int,
                                        medida nvarchar(10),
                                        cantidadAdicional Int);
                                        
    CREATE TEMPORARY TABLE tbSubcomponente (id Int, idSubComponente Int);
    INSERT tbSubcomponente (id, idSubComponente)
	SELECT componentes_detalle.id, Id_subcomponente FROM  componentes_detalle 
    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
    WHERE componentes.Codigo = pCodigo;
    
    SET count = (SELECT count(*) FROM tbSubcomponente);

    WHILE contador < count DO
    
	SET idv = (SELECT id FROM tbSubcomponente order by id LIMIT contador,1);

    SET idSubComponentev = (SELECT idSubComponente FROM tbSubcomponente order by id LIMIT contador,1);

    SET idUnidadCalculada =  (SELECT  id_Unidad_calculada FROM componentes_detalle
								JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
								WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
                                
    IF (idUnidadCalculada = 1) THEN -- longitud recopilada
    
       SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle
								JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                                WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       
       IF  (aplicaDecremento = 0) THEN 
       
	     SET pmedida = plogitud + (SELECT cantidad_adicional FROM componentes_detalle 
                                     JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                                     WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       ELSE 
	     SET pmedida = plogitud - (SELECT cantidad_adicional FROM componentes_detalle 
                                    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
									WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       END IF;
       

      INSERT tableResult 
      SELECT subcomponentes.id_subcomponente,
              idUnidadCalculada,
	          CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
             subcomponentes.descripcion,
             Cantidad_default AS cantidad, 
			 CEILING(pmedida * Cantidad_default) AS medida,
             3000
     FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
     JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
	 JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
	 WHERE componentes_detalle.id = idv
		AND componentes.codigo = pCodigo;
    
    ELSEIF (idUnidadCalculada = 2) THEN  -- unidad
 
         INSERT tableResult 
         SELECT  subcomponentes.id_subcomponente,
                 idUnidadCalculada,
				 CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
				 subcomponentes.descripcion,
				 CEILING((plogitud * cantidad_default)/1000) AS cantidad, 
				 "" AS medida,
                 0
		FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
		JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
        JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		WHERE componentes_detalle.id = idv
		AND componentes.codigo = pCodigo;
        
	ELSEIF (idUnidadCalculada = 3) THEN  -- cantidad
    
		 INSERT tableResult 
         SELECT  subcomponentes.id_subcomponente,
                 idUnidadCalculada,
				CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
				 subcomponentes.descripcion,
				 cantidad_default AS cantidad, 
				 "" AS medida,
                 0
		FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
		JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
         JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		WHERE componentes_detalle.id = idv
		AND componentes.codigo = pCodigo;
        
	ELSEIF (idUnidadCalculada = 4) THEN -- medida Exacta
    
	 CREATE TEMPORARY TABLE tbCantidad (idSubcomponente Int, cantidad int);
		INSERT tbCantidad (idSubcomponente, cantidad)
		SELECT  Id_Subcomponente, cantidad_default 
          FROM  componentes_detalle 
          JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
		  WHERE componentes.Codigo = pCodigo;
          
		 SET cantidad_U = (SELECT sum(cantidad) FROM tbCantidad WHERE tbCantidad.idSubcomponente = idSubComponente);
         
		 INSERT tableResult 
         SELECT DISTINCT  subcomponentes.id_subcomponente,
						  idUnidadCalculada,
				          CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
				          subcomponentes.descripcion,
						  cantidad_U AS cantidad, 
				          "" AS medida,
                          cantidad_adicional
		FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
		JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
         JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		WHERE componentes_detalle.id = idv
		  AND componentes.codigo = pCodigo;
	
         if (SELECT count(*) FROM tbSubcomponente WHERE tbSubcomponente.idSubcomponente = idSubComponente) > 1 THEN
            DELETE FROM tbSubcomponente WHERE tbSubcomponente.idSubcomponente = idSubComponentev;
            SET count = count - (SELECT count(*) FROM tbCantidad WHERE idSubcomponente = idSubComponentev);
         END IF;
         
         DROP TABLE tbCantidad;
	ELSEIF (idUnidadCalculada = 5) THEN  -- longitud sin recopilar
    
	  SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle
							    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
							    WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       
       IF  (aplicaDecremento = 0) THEN 
	     SET pmedida = plogitud + (SELECT cantidad_adicional FROM componentes_detalle 
                                     JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                                     WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       ELSE 
	     SET pmedida = plogitud - (SELECT cantidad_adicional FROM componentes_detalle 
                                    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
									 WHERE componentes_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       END IF;
       
       CREATE TEMPORARY TABLE tbLongitudSinRecopilar (idSubcomponente Int, cantidad int);
	    INSERT tbLongitudSinRecopilar (idSubcomponente, cantidad)
		SELECT  Id_Subcomponente, cantidad_default 
          FROM  componentes_detalle 
          JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
		  WHERE componentes_detalle.id = idv
		  AND componentes.Codigo = pCodigo;
          
          
          SET cantidad_U = (SELECT sum(cantidad) FROM tbLongitudSinRecopilar WHERE tbLongitudSinRecopilar.idSubcomponente = idSubComponente);
          
		 INSERT tableResult 
         SELECT DISTINCT  subcomponentes.id_subcomponente,
						  idUnidadCalculada,
				          CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
				          subcomponentes.descripcion,
						  cantidad_U, 
                          0,
				          pmedida
		FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
		JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
         JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		WHERE componentes_detalle.id = idv
		  AND componentes.codigo = pCodigo;
        
		 if (SELECT count(*) FROM tbSubcomponente WHERE tbSubcomponente.idSubcomponente = idSubComponentev) > 1 THEN
            DELETE FROM tbSubcomponente WHERE tbSubcomponente.idSubcomponente = idSubComponentev and tbSubcomponente.id = idv ;
            SET count = count - (SELECT count(*) FROM tbLongitudSinRecopilar WHERE idSubcomponente = idSubComponentev);
         END IF;
         
		DROP TABLE tbLongitudSinRecopilar;
    END IF; 
    
    SET contador = contador + 1;
    
 END WHILE;  
 
   SELECT id_subcomponente, 
          id_unidad_Calculada,
          codigo , 
          descripcion ,
          cantidad ,
          cantidadAdicional AS medidaBase,
          medida
	FROM tableResult;
   
   DROP TABLE tableResult;
   DROP TABLE tbSubcomponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteVidrioPanel` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteVidrioPanel`(
 pCodigo    nvarchar(50),
 pAltura    float
)
BEGIN
	DECLARE idv Int;
    DECLARE count INT;
    DECLARE countColumns INT;
    DECLARE selectColumna INT;
    DECLARE contador INT default 0;
	DECLARE contadorColumns INT default 0;
    DECLARE cantidad Int;
	DECLARE idUnidadCalculada INT; 
	DECLARE aplicaDecremento BIT;
    DECLARE pmedida float;
    DECLARE col,alt INT;

	CREATE TEMPORARY TABLE tableResult (id_subcomponente Int, 
                                        id_unidad_Calculada Int,
                                        codigo nvarchar(10), 
                                        descripcion nvarchar(100),
                                        altura nvarchar(100),
                                        anchura nvarchar(100),
                                        cantidad Int,
                                        medida nvarchar(10),
                                        cantidadAdicional Int);
                                        
	CREATE TEMPORARY TABLE tbSubcomponente (id Int);
    
    INSERT tbSubcomponente (id)
	SELECT componentes_especial_detalle.id FROM  componentes_especial_detalle 
    JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes_especial.Id_Componente_especial
    WHERE componentes_especial.Codigo = pCodigo;
    
    SET count = (SELECT count(*) FROM tbSubcomponente);   

    WHILE contador < count DO
    
       	SET idv = (SELECT * FROM tbSubcomponente order by id LIMIT contador,1);
    
        SET selectColumna =  (SELECT select_Columna FROM componentes_especial_detalle
								JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes_especial.Id_Componente_especial
								WHERE id = idv AND codigo = pCodigo LIMIT 0,1);
		 
		SET countColumns = (SELECT count(*) FROM tbauxanchura);

        -- SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);
        
        IF selectColumna = 1  THEN 

           SET contadorColumns = 0;
           
           WHILE contadorColumns < countColumns DO
           
				SET col = (SELECT Columna1 FROM tbauxanchura  LIMIT contadorColumns,1);
                
				SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);
                 
                INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);
                SET contadorColumns = contadorColumns + 1;
           END WHILE;
           
        END IF;
        
		IF selectColumna = 2  THEN 
        
		sET contadorColumns = 0;
          
		WHILE contadorColumns < countColumns DO
        
				SET col = (SELECT Columna2 FROM tbauxanchura LIMIT contadorColumns,1);
                
                SET alt = (SELECT Altura FROM tbauxanchura LIMIT contadorColumns,1);
                
                INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);
                
                SET contadorColumns = contadorColumns + 1;
                
		END WHILE;
        
        END IF;
        
		IF selectColumna = 3  THEN 
        
            SET contadorColumns = 0;
            
           WHILE contadorColumns < countColumns DO
           
				SET col = (SELECT Columna3 FROM tbauxanchura LIMIT contadorColumns,1);
                
                SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);
                
                INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);
                
                SET contadorColumns = contadorColumns + 1;
           END WHILE;
           
        END IF;
        
        IF selectColumna = 4  THEN 

		   SET contadorColumns = 0;
           
           WHILE contadorColumns < countColumns DO
           
				SET col = (SELECT Columna4 FROM tbauxanchura LIMIT contadorColumns,1);
                
                SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);
                
                INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);
                
                SET contadorColumns = contadorColumns + 1;
                
           END WHILE;
           
        END IF;
        
        IF selectColumna = 5  THEN
        
           SET contadorColumns = 0;
           
           WHILE contadorColumns < countColumns DO
           
				SET col = (SELECT Columna5 FROM tbauxanchura  LIMIT contadorColumns,1);
                
                SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);
                
                INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);
                
                SET contadorColumns = contadorColumns + 1;
                
           END WHILE;
           
        END IF;
        
		IF selectColumna = 6  THEN
	    SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_especial_detalle
								JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes_especial.Id_Componente_especial
                                WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
       
         IF  (aplicaDecremento = 0) THEN 
       
	        SET pmedida = plogitud + (SELECT cantidad_adicional FROM componentes_especial_detalle 
                                        JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente_especial
									   WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
	     ELSE 
	       SET pmedida = plogitud - (SELECT cantidad_adicional FROM componentes_especial_detalle 
                                       JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente_especial
									  WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);
		END IF;
       

	/*	INSERT tableResult 
		SELECT subcomponentes.id_subcomponente,
				  selectColumna,
				  CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
				  subcomponentes.descripcion,
                  "N/A",
                  "N/A",
				  Cantidad_default AS cantidad, 
				  CEILING(pmedida * Cantidad_default) AS medida,
				  3000
		 FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
		 JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
		 JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		 WHERE componentes_detalle.id = idv
		 AND componentes.codigo = pCodigo;*/
         
	 END IF;
     
	 SET contador = contador + 1;
     
	INSERT tableResult
	SELECT Codigo id,
		   selectColumna,
		   CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,
		   subcomponentes.descripcion,
		   Altura,
		   Anchura,
		   count(*) cantidad,
		   0,
		   cantidad_adicional
		FROM proyecto_vidriopanel JOIN componentes_especial_detalle 
		ON proyecto_vidriopanel.Codigo = componentes_especial_detalle.id
		JOIN subcomponentes ON subcomponentes.Id_Subcomponente = componentes_especial_detalle.Id_Subcomponente
		JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
		group by proyecto_vidriopanel.Codigo,selectColumna,Altura,Anchura;
        
    END WHILE;  
    
	 SELECT * FROM tableResult 
	 ORDER BY codigo DESC;

	 TRUNCATE proyecto_vidriopanel; 
	 DROP TABLE tbSubcomponente;
	 DROP TABLE tableResult;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteVidrioPanelInsert` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteVidrioPanelInsert`(
	pCodigo  nvarchar(50),
	pAltura  int,
	pAchura  Int 
)
BEGIN

  INSERT proyecto_vidriopanel (Codigo,
                               Altura,
                               Anchura) 
                                     VALUES(pCodigo, 
                                             pAltura,
                                             pAchura);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteAgrupar`()
BEGIN
  SET SQL_SAFE_UPDATES = 0;
  
   SELECT proy.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
        subcomponente.Descripcion,
        CASE WHEN proy.Id_Unidad_Medida != 1 THEN (SELECT SUM(proyect.cantidad) FROM arquitectdb.proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional)
        ELSE ceiling((SELECT SUM(proyect.medida) FROM arquitectdb.proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / 3000) END cantidad,
		proy.medidaAdicional medidaC,
        CASE WHEN proy.Id_Unidad_Medida != 1 THEN 0
        ELSE (SELECT SUM(proyect.medida) FROM arquitectdb.proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) END medidaCalculada,
        fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida
  FROM arquitectdb.proyecto proy 
  JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
  JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
  GROUP BY proy.Id_Subcomponente, proy.Id_Unidad_Medida, proy.medidaAdicional ;
  
   DELETE FROM proyecto;
  
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubcomponenteCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubcomponenteCargar`(
 idSubComponente int
)
BEGIN
   SELECT subcomponente.Id_Acabado,
          subcomponente.Codigo_Homologacion,
          subcomponente.Descripcion,
          acabado.Codigo_Homologacion codigo,
          Especial
   FROM subcomponentes subcomponente 
   JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
   WHERE Id_Subcomponente = idSubComponente;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteConsultar`(
 pCadena nvarchar(300)
)
BEGIN
   SELECT Id_subcomponente, 
          CONCAT(subcomponentes.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion)  codigo,
          subcomponentes.Descripcion 
    FROM arquitectdb.subcomponentes JOIN arquitectdb.acabados 
    ON subcomponentes.Id_Acabado = acabados.Id_Acabado
    WHERE CONCAT(subcomponentes.Codigo_Homologacion,"-",subcomponentes.Descripcion)
    lIKE concat('%',  pCadena , '%');
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteEspecialConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteEspecialConsultar`(
   pCadena nvarchar(300)
)
BEGIN
     SELECT Id_subcomponente, 
          CONCAT(subcomponentes.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion)  codigo,
          subcomponentes.Descripcion 
    FROM arquitectdb.subcomponentes JOIN arquitectdb.acabados 
    ON subcomponentes.Id_Acabado = acabados.Id_Acabado
    WHERE CONCAT(subcomponentes.Codigo_Homologacion,"-",subcomponentes.Descripcion)
    lIKE concat('%',  pCadena , '%') AND subcomponentes.Especial = true;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteMamparaAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteMamparaAgrupar`()
BEGIN

   SELECT codigo,
          descripcion,
          SUM(medida) medida
   FROM proyecto_mp GROUP BY codigo;
   
   DELETE FROM proyecto_mp;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteRegistrar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteRegistrar`(
pId_Acabado int,
pCodigo nvarchar(10),
pDescripcion nvarchar(100),
pEspecial  boolean,
 pIdComponente int
)
BEGIN
   INSERT subcomponentes (Id_Acabado,
                          Codigo_Homologacion,
                          Descripcion,Especial) VALUES (pId_Acabado,
                                                      pCodigo,
                                                      pDescripcion,pEspecial
                                                      );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubcomponenteUpdate` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubcomponenteUpdate`(
 pIdAcabdo int, 
 pCodigo nvarchar(50),
 pDescripcion  nvarchar(300),
 pIdComponente int,
 pEspecial boolean
)
BEGIN
    UPDATE `arquitectdb`.`subcomponentes`
SET
	`Id_Acabado` =pIdAcabdo,
	`Codigo_Homologacion` = pCodigo,
	`Descripcion` = pDescripcion,
     `Especial` = pEspecial
    WHERE `Id_Subcomponente` = pIdComponente;
    
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteVidrioPanelAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteVidrioPanelAgrupar`()
BEGIN

  SET SQL_SAFE_UPDATES = 0;
  
   SELECT componentes_especial_detalle.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
        subcomponente.Descripcion,
        proy.Altura,
        proy.Anchura,
		CASE WHEN proy.Id_Unidad_Medida != 6 THEN proy.cantidad
        ELSE ceiling((SELECT SUM(proyect.medida) FROM arquitectdb.proyecto_vp proyect 
        WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente 
        and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / 3000) END cantidad,
		proy.medidaAdicional medidaC,
        CASE WHEN proy.Id_Unidad_Medida != 6 THEN 0
        ELSE (SELECT SUM(proyect.medida) FROM arquitectdb.proyecto_vp proyect 
        WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente 
        and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) END medidaCalculada,
        fnUnidadCalculadaVidrioPanel(proy.Id_Unidad_Medida) unidaMedida
  FROM arquitectdb.proyecto_vp proy
  JOIN componentes_especial_detalle ON componentes_especial_detalle.id = proy.Id_Subcomponente 
  JOIN subcomponentes subcomponente ON componentes_especial_detalle.Id_Subcomponente = subcomponente.Id_Subcomponente
  JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
  WHERE Anchura <> 0
  GROUP BY componentes_especial_detalle.Id_Subcomponente,proy.Id_Unidad_Medida, altura, anchura;
  
  DELETE FROM proyecto_vp;
  
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-06-14 12:52:39
