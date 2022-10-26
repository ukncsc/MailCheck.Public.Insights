ALTER TABLE `insights`.`aggregated_configuration` 
ADD COLUMN `dmarc_passed_count` BIGINT(20) NULL DEFAULT NULL AFTER `dkim_failed_count`;