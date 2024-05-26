-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 26, 2024 at 05:57 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `uaspemvis`
--

-- --------------------------------------------------------

--
-- Table structure for table `tabel_data`
--

CREATE TABLE `tabel_data` (
  `nim` int(11) NOT NULL,
  `nama` varchar(255) NOT NULL,
  `jenis_kelamin` varchar(255) NOT NULL,
  `prodi` varchar(255) NOT NULL,
  `pernah_mengulang` enum('ya','tidak') NOT NULL,
  `ipk` float NOT NULL,
  `gambar` blob NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tabel_data`
--

INSERT INTO `tabel_data` (`nim`, `nama`, `jenis_kelamin`, `prodi`, `pernah_mengulang`, `ipk`, `gambar`) VALUES
(65, 'Je', 'Perempuan', 'Teknik Informatikan', 'ya', 3.25, ''),
(66, 'wa', 'Laki-laki', 'Teknik Informatika', 'tidak', 3.75, ''),
(67, 'A', 'Perempuan', 'Teknik Informatika', 'tidak', 3.5, '');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tabel_data`
--
ALTER TABLE `tabel_data`
  ADD PRIMARY KEY (`nim`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
