ALTER TABLE `insights`.`aggregated_configuration` 
ADD COLUMN `host_name` varchar(255) NULL DEFAULT NULL AFTER `provider`;