-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 26-04-2025 a las 17:27:31
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `neomarket`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `buyout`
--

CREATE TABLE `buyout` (
  `Id` int(11) NOT NULL,
  `Quantity` text NOT NULL,
  `Date` datetime NOT NULL,
  `IdUser` int(11) NOT NULL,
  `IdProduct` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `category`
--

CREATE TABLE `category` (
  `Id` int(11) NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `company`
--

CREATE TABLE `company` (
  `Id` int(11) NOT NULL,
  `NameCompany` text NOT NULL,
  `PhoneCompany` int(11) NOT NULL,
  `EmailCompany` text NOT NULL,
  `NitCompany` smallint(6) NOT NULL,
  `Logo` text NOT NULL,
  `Description` text NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `CreateAt` datetime NOT NULL,
  `DeleteAt` datetime NOT NULL,
  `UpdateAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `company`
--

INSERT INTO `company` (`Id`, `NameCompany`, `PhoneCompany`, `EmailCompany`, `NitCompany`, `Logo`, `Description`, `Status`, `CreateAt`, `DeleteAt`, `UpdateAt`) VALUES
(1, 'hjj', 25429557, 'strasdaing', 0, 'sas', 'kk', 1, '2025-04-23 20:41:13', '2025-04-23 20:41:13', '2025-04-23 20:41:13');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `form`
--

CREATE TABLE `form` (
  `Id` int(11) NOT NULL,
  `NameForm` text NOT NULL,
  `Description` text NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `IdModule` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imageproduct`
--

CREATE TABLE `imageproduct` (
  `Id` int(11) NOT NULL,
  `UrlImage` text NOT NULL,
  `Status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inventory`
--

CREATE TABLE `inventory` (
  `Id` int(11) NOT NULL,
  `NameInventory` text NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `DescriptionInventory` text NOT NULL,
  `Observation` text NOT NULL,
  `ZoneProduct` text NOT NULL,
  `CreateAt` datetime NOT NULL,
  `DeleteAt` datetime NOT NULL,
  `UpdateAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `module`
--

CREATE TABLE `module` (
  `Id` int(11) NOT NULL,
  `NameModule` text NOT NULL,
  `Status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `movimientoinventory`
--

CREATE TABLE `movimientoinventory` (
  `Id` int(11) NOT NULL,
  `TypeMovement` enum('Entry','Exit') NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Date` datetime NOT NULL,
  `Description` text NOT NULL,
  `IdInventory` int(11) NOT NULL,
  `IdUser` int(11) NOT NULL,
  `IdProduct` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `notification`
--

CREATE TABLE `notification` (
  `Id` int(11) NOT NULL,
  `TypeAction` enum('Sale','Buy','Movement') NOT NULL,
  `IdReferece` text NOT NULL,
  `Mensage` text NOT NULL,
  `Read` tinyint(1) NOT NULL,
  `Date` datetime NOT NULL,
  `IdUser` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `person`
--

CREATE TABLE `person` (
  `Id` int(11) NOT NULL,
  `FirstName` text NOT NULL,
  `LastName` text NOT NULL,
  `PhoneNumber` text NOT NULL,
  `Email` text NOT NULL,
  `TypeIdentification` enum('CC','CE','TI','RC','NIT','PASSPORT') NOT NULL,
  `NumberIdentification` int(11) NOT NULL,
  `Status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `person`
--

INSERT INTO `person` (`Id`, `FirstName`, `LastName`, `PhoneNumber`, `Email`, `TypeIdentification`, `NumberIdentification`, `Status`) VALUES
(1, 'Diego', 'Alvarado', '3177312500', 'daalvardo@gmail.com', 'TI', 1076906609, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `product`
--

CREATE TABLE `product` (
  `Id` int(11) NOT NULL,
  `NameProduct` text NOT NULL,
  `Description` text NOT NULL,
  `Price` text NOT NULL,
  `CreateAt` datetime NOT NULL,
  `DeleteAt` datetime NOT NULL,
  `UpdateAt` datetime NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `IdInventory` int(11) NOT NULL,
  `IdImageItem` int(11) NOT NULL,
  `IdCategory` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `rol`
--

CREATE TABLE `rol` (
  `Id` int(11) NOT NULL,
  `NameRol` text NOT NULL,
  `Description` text NOT NULL,
  `Status` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `rol`
--

INSERT INTO `rol` (`Id`, `NameRol`, `Description`, `Status`) VALUES
(1, 'Admin', 'adiministrador', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `rolform`
--

CREATE TABLE `rolform` (
  `Id` int(11) NOT NULL,
  `Permission` enum('Create','Read','Update','Delete') NOT NULL,
  `IdForm` int(11) NOT NULL,
  `IdRol` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `sale`
--

CREATE TABLE `sale` (
  `Id` int(11) NOT NULL,
  `Date` datetime NOT NULL,
  `totaly` text NOT NULL,
  `IdUser` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `saledetail`
--

CREATE TABLE `saledetail` (
  `Id` int(11) NOT NULL,
  `Quantity` text NOT NULL,
  `Price` text NOT NULL,
  `IdSale` int(11) NOT NULL,
  `IdProduct` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `sede`
--

CREATE TABLE `sede` (
  `Id` int(11) NOT NULL,
  `NameSede` text NOT NULL,
  `AddressSede` text NOT NULL,
  `PhoneSede` smallint(6) NOT NULL,
  `EmailSede` text NOT NULL,
  `CodeSede` int(11) NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `CreateAt` datetime NOT NULL,
  `DeleteAt` datetime NOT NULL,
  `UpdateAt` datetime NOT NULL,
  `IdCompany` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `user`
--

CREATE TABLE `user` (
  `Id` int(11) NOT NULL,
  `Username` text NOT NULL,
  `Password` text NOT NULL,
  `CreateAt` datetime NOT NULL,
  `DeleteAt` datetime NOT NULL,
  `Status` tinyint(1) NOT NULL,
  `UpdateAt` datetime NOT NULL,
  `IdCompany` int(11) NOT NULL,
  `IdPerson` int(11) NOT NULL,
  `IdRol` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `user`
--

INSERT INTO `user` (`Id`, `Username`, `Password`, `CreateAt`, `DeleteAt`, `Status`, `UpdateAt`, `IdCompany`, `IdPerson`, `IdRol`) VALUES
(1, 'string', 'string', '2025-04-25 01:37:06', '2025-04-25 01:37:06', 1, '2025-04-25 01:37:06', 1, 1, 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `buyout`
--
ALTER TABLE `buyout`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdUser` (`IdUser`),
  ADD KEY `IdProduct` (`IdProduct`);

--
-- Indices de la tabla `category`
--
ALTER TABLE `category`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `company`
--
ALTER TABLE `company`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `form`
--
ALTER TABLE `form`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdModule` (`IdModule`);

--
-- Indices de la tabla `imageproduct`
--
ALTER TABLE `imageproduct`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `inventory`
--
ALTER TABLE `inventory`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `module`
--
ALTER TABLE `module`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `movimientoinventory`
--
ALTER TABLE `movimientoinventory`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdInventory` (`IdInventory`),
  ADD KEY `IdUser` (`IdUser`),
  ADD KEY `IdProduct` (`IdProduct`);

--
-- Indices de la tabla `notification`
--
ALTER TABLE `notification`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdUser` (`IdUser`);

--
-- Indices de la tabla `person`
--
ALTER TABLE `person`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdInventory` (`IdInventory`),
  ADD KEY `IdImageItem` (`IdImageItem`),
  ADD KEY `IdCategory` (`IdCategory`);

--
-- Indices de la tabla `rol`
--
ALTER TABLE `rol`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `rolform`
--
ALTER TABLE `rolform`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdForm` (`IdForm`),
  ADD KEY `IdRol` (`IdRol`);

--
-- Indices de la tabla `sale`
--
ALTER TABLE `sale`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdUser` (`IdUser`);

--
-- Indices de la tabla `saledetail`
--
ALTER TABLE `saledetail`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdSale` (`IdSale`),
  ADD KEY `IdProduct` (`IdProduct`);

--
-- Indices de la tabla `sede`
--
ALTER TABLE `sede`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdCompany` (`IdCompany`);

--
-- Indices de la tabla `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdPerson` (`IdPerson`),
  ADD KEY `IdCompany` (`IdCompany`),
  ADD KEY `IdRol` (`IdRol`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `buyout`
--
ALTER TABLE `buyout`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `category`
--
ALTER TABLE `category`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `company`
--
ALTER TABLE `company`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `form`
--
ALTER TABLE `form`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `imageproduct`
--
ALTER TABLE `imageproduct`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inventory`
--
ALTER TABLE `inventory`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `module`
--
ALTER TABLE `module`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `movimientoinventory`
--
ALTER TABLE `movimientoinventory`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `notification`
--
ALTER TABLE `notification`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `person`
--
ALTER TABLE `person`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `product`
--
ALTER TABLE `product`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `rol`
--
ALTER TABLE `rol`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `rolform`
--
ALTER TABLE `rolform`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `sale`
--
ALTER TABLE `sale`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `saledetail`
--
ALTER TABLE `saledetail`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `sede`
--
ALTER TABLE `sede`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `user`
--
ALTER TABLE `user`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `buyout`
--
ALTER TABLE `buyout`
  ADD CONSTRAINT `buyout_ibfk_1` FOREIGN KEY (`IdUser`) REFERENCES `user` (`Id`),
  ADD CONSTRAINT `buyout_ibfk_2` FOREIGN KEY (`IdProduct`) REFERENCES `product` (`Id`);

--
-- Filtros para la tabla `form`
--
ALTER TABLE `form`
  ADD CONSTRAINT `form_ibfk_1` FOREIGN KEY (`IdModule`) REFERENCES `module` (`Id`);

--
-- Filtros para la tabla `movimientoinventory`
--
ALTER TABLE `movimientoinventory`
  ADD CONSTRAINT `movimientoinventory_ibfk_1` FOREIGN KEY (`IdInventory`) REFERENCES `inventory` (`Id`),
  ADD CONSTRAINT `movimientoinventory_ibfk_2` FOREIGN KEY (`IdUser`) REFERENCES `user` (`Id`),
  ADD CONSTRAINT `movimientoinventory_ibfk_3` FOREIGN KEY (`IdProduct`) REFERENCES `product` (`Id`);

--
-- Filtros para la tabla `notification`
--
ALTER TABLE `notification`
  ADD CONSTRAINT `notification_ibfk_1` FOREIGN KEY (`IdUser`) REFERENCES `user` (`Id`);

--
-- Filtros para la tabla `product`
--
ALTER TABLE `product`
  ADD CONSTRAINT `product_ibfk_1` FOREIGN KEY (`IdInventory`) REFERENCES `inventory` (`Id`),
  ADD CONSTRAINT `product_ibfk_2` FOREIGN KEY (`IdImageItem`) REFERENCES `imageproduct` (`Id`),
  ADD CONSTRAINT `product_ibfk_3` FOREIGN KEY (`IdCategory`) REFERENCES `category` (`Id`);

--
-- Filtros para la tabla `rolform`
--
ALTER TABLE `rolform`
  ADD CONSTRAINT `rolform_ibfk_1` FOREIGN KEY (`IdForm`) REFERENCES `form` (`Id`),
  ADD CONSTRAINT `rolform_ibfk_2` FOREIGN KEY (`IdRol`) REFERENCES `rol` (`Id`);

--
-- Filtros para la tabla `sale`
--
ALTER TABLE `sale`
  ADD CONSTRAINT `sale_ibfk_1` FOREIGN KEY (`IdUser`) REFERENCES `user` (`Id`);

--
-- Filtros para la tabla `saledetail`
--
ALTER TABLE `saledetail`
  ADD CONSTRAINT `saledetail_ibfk_1` FOREIGN KEY (`IdSale`) REFERENCES `sale` (`Id`),
  ADD CONSTRAINT `saledetail_ibfk_2` FOREIGN KEY (`IdProduct`) REFERENCES `product` (`Id`);

--
-- Filtros para la tabla `sede`
--
ALTER TABLE `sede`
  ADD CONSTRAINT `sede_ibfk_1` FOREIGN KEY (`IdCompany`) REFERENCES `company` (`Id`);

--
-- Filtros para la tabla `user`
--
ALTER TABLE `user`
  ADD CONSTRAINT `user_ibfk_1` FOREIGN KEY (`IdPerson`) REFERENCES `person` (`Id`),
  ADD CONSTRAINT `user_ibfk_2` FOREIGN KEY (`IdCompany`) REFERENCES `company` (`Id`),
  ADD CONSTRAINT `user_ibfk_3` FOREIGN KEY (`IdRol`) REFERENCES `rol` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
